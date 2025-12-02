using System.CommandLine;

namespace AdventOfCode.Cli;

public static class Program
{
    static async Task<int> Main(string[] args)
    {
        // Create the root command for the CLI tool
        var rootCommand = new RootCommand("Advent of Code Solver");

        // Define a "run" subcommand for executing solutions
        var runCommand = new Command("run", "Run Advent of Code solutions");

        // Add options to the "run" command
        var yearOption = new Option<ushort>("--year", "-y") { Description = "Run solutions for a specific year" };
        var dayOption = new Option<ushort>("--day", "-d") { Description = "Run solutions for the day" };
        runCommand.Options.Add(yearOption);
        runCommand.Options.Add(dayOption);

        // Add validation logic to ensure correct usage of the command
        runCommand.Validators.Add(result =>
        {
            var year = result.GetResult(yearOption);
            var day = result.GetResult(dayOption);

            // If day is specified but year is not, add an error
            if (day is { Tokens.Count: 1 } && year is not { Tokens.Count: 1 })
            {
                result.AddError("--day requires --year. Please specify --year <value>.");
                return;
            }

            // If year is not specified, no further validation is needed
            if (year is not { Tokens.Count: 1 }) return;

            // Get the numeric value of the year
            var inputYear = result.GetValue(yearOption);

            // If the specified year does not have solutions, add an error
            if (!Runner.SolutionDescriptors
                    .Select(descriptor => descriptor.Year)
                    .Contains(inputYear))
            {
                result.AddError($"No solutions exist for year {inputYear}.");
                return;
            }

            // If day is not specified, no further validation is needed
            if (day is not { Tokens.Count: 1 }) return;

            // Get the numeric value of the day
            var inputDay = result.GetValue(dayOption);

            // If the specified day does not exist for the year, add an error
            if (!Runner.SolutionDescriptors
                    .Where(descriptor => descriptor.Year == inputYear)
                    .Select(descriptor => descriptor.Day)
                    .Contains(inputDay))
                result.AddError($"No solutions exist for day {inputDay} in year {inputYear}.");
        });

        // Define the action to execute when the "run" command is invoked
        runCommand.SetAction(parseResult =>
        {
            var year = parseResult.GetValue(yearOption);
            var day = parseResult.GetValue(dayOption);
            var result = Runner.TryRun(year, day);
            return result ? 0 : 1;
        });

        // Add the "run" subcommand to the root command
        rootCommand.Subcommands.Add(runCommand);

        // Parse the command-line arguments
        var parseResult = rootCommand.Parse(args);

        // Invoke the parsed command asynchronously
        return await parseResult.InvokeAsync();
    }
}