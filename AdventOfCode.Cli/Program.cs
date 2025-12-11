using System.CommandLine;

namespace AdventOfCode.Cli;

public static class Program
{
    static async Task<int> Main(string[] args)
    {
        // Root command for the CLI tool; displays this description in help output.
        var rootCommand = new RootCommand("Advent of Code Solver");

        // Subcommand responsible for running one or more AoC solutions.
        var runCommand = new Command("run", "Run Advent of Code solutions");

        // Options that determine which solution(s) to run.
        var yearOption = new Option<string>("--year", "-y")
        {
            Description = "Run solutions for the specified year",
        };

        var dayOption = new Option<string>("--day", "-d")
        {
            Description = "Run the solution for a specific day (requires --year)",
        };

        var exampleOption = new Option<bool>("--example", "-e")
        {
            Description = "Use example input instead of the real puzzle input",
        };

        runCommand.Options.Add(yearOption);
        runCommand.Options.Add(dayOption);
        runCommand.Options.Add(exampleOption);

        // Validation logic that enforces correct argument combinations
        // and ensures referenced solutions actually exist.
        runCommand.Validators.Add(result =>
        {
            var year = result.GetValue(yearOption);
            var day = result.GetValue(dayOption);

            // --day cannot be used without --year
            if (day is not null && year is null)
            {
                result.AddError("--day requires --year. Please specify --year <value>.");
                return;
            }

            // If no --year is provided, nothing else to validate.
            if (year is null)
                return;

            // Validate that --year is a valid number
            if (!ushort.TryParse(year, out var inputYear))
            {
                result.AddError("--year must be a number.");
                return;
            }

            // Ensure the specified year exists in the detected solution set
            if (
                !Runner
                    .SolutionDescriptors.Select(descriptor => descriptor.Year)
                    .Contains(inputYear)
            )
            {
                result.AddError($"No solutions exist for year {inputYear}.");
                return;
            }

            // If --day is omitted, validation is complete
            if (day is null)
                return;

            // Validate --day is numeric
            if (!ushort.TryParse(day, out var inputDay))
            {
                result.AddError("--day must be a number.");
                return;
            }

            // Ensure the specified day exists for the given year
            if (
                !Runner
                    .SolutionDescriptors.Where(descriptor => descriptor.Year == inputYear)
                    .Select(descriptor => descriptor.Day)
                    .Contains(inputDay)
            )
            {
                result.AddError($"No solutions exist for day {inputDay} in year {inputYear}.");
            }
        });

        // Define what to execute when "run" is invoked and validation succeeded.
        runCommand.SetAction(parseResult =>
        {
            ushort? year = parseResult.GetValue(yearOption) is { } yearString
                ? ushort.Parse(yearString)
                : null;
            ushort? day = parseResult.GetValue(dayOption) is { } dayString
                ? ushort.Parse(dayString)
                : null;
            var useExample = parseResult.GetValue(exampleOption);
            Runner.Run(year, day, useExample);
        });

        // Attach "run" as a subcommand of the root.
        rootCommand.Subcommands.Add(runCommand);

        // Parse the incoming CLI arguments.
        var parseResult = rootCommand.Parse(args);

        // Execute the command asynchronously.
        return await parseResult.InvokeAsync();
    }
}
