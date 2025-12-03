using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;
using AdventOfCode.Solutions;

namespace AdventOfCode.Cli;

/// <summary>
/// Describes a solution by its year, day, and implementing type.
/// </summary>
public readonly record struct SolutionDescriptor(ushort Year, ushort Day, Type Type);

public static class Runner
{
    /// <summary>
    /// Represents the outcome of running a single solution part (Part 1 or Part 2).
    /// </summary>
    private readonly record struct ExecutionResult(
        ushort Year,
        ushort Day,
        ushort Part,
        bool Success,
        string? Output,
        Exception? Exception
    );

    /// <summary>
    /// Reference to the solution's assembly.
    /// </summary>
    private static readonly Assembly SolutionAssembly = typeof(ISolution).Assembly;

    /// <summary>
    /// Regex used to extract year and day information from namespaces.
    /// Example namespace: AdventOfCode.Solutions.Year2024.Day01
    /// </summary>
    private static readonly Regex SolutionRegex =
        new(@"^AdventOfCode\.Solutions\.Year(?<year>\d{4})\.Day(?<day>\d{2})$");

    /// <summary>
    /// Gets a list of all available solutions in the assembly.
    /// This list is lazily evaluated, cached, and sorted by Year then Day.
    /// </summary>
    public static ImmutableList<SolutionDescriptor> SolutionDescriptors
    {
        get
        {
            // Return cached value if already initialized
            if (field != null) return field;

            var builder = ImmutableList.CreateBuilder<SolutionDescriptor>();

            // Scan all types in the solutions assembly
            foreach (var type in SolutionAssembly.GetTypes())
            {
                // Only consider concrete classes that implement ISolution
                if (!typeof(ISolution).IsAssignableFrom(type) || !type.IsClass)
                    continue;

                // Must have a namespace to match
                var ns = type.Namespace;
                if (ns is null)
                    continue;

                // Namespace determines year/day via regex
                var match = SolutionRegex.Match(ns);
                if (!match.Success)
                    continue;

                // Parse year/day from regex captures
                if (!ushort.TryParse(match.Groups["year"].Value, out var parsedYear))
                    continue;
                if (!ushort.TryParse(match.Groups["day"].Value, out var parsedDay))
                    continue;

                builder.Add(new SolutionDescriptor(parsedYear, parsedDay, type));
            }

            // Cache immutable sorted result
            field = builder
                .OrderBy(d => d.Year)
                .ThenBy(d => d.Day)
                .ToImmutableList();

            return field;
        }
    }

    /// <summary>
    /// Runs all selected solutions, executes both parts, and prints results.
    /// </summary>
    public static void Run(ushort? year, ushort? day)
    {
        // Determine which solutions to run based on provided filters
        var descriptors = SolutionDescriptors.Where(descriptor =>
                year is null && day is null || // Run all solutions
                day is null && descriptor.Year == year || // Run all for a given year
                descriptor.Year == year && descriptor.Day == day // Run a specific solution
        ).ToList();

        // Instantiate solution objects
        var solutions = LoadSolutions(descriptors)
            .Zip(descriptors, (solution, descriptor) => (solution, descriptor));

        // Execute solutions in order
        foreach (var (solution, descriptor) in solutions)
        {
            try
            {
                // Load the input for this solution
                var input = LoadInput(descriptor.Year, descriptor.Day);

                // Execute both parts of the solution
                foreach (var result in ExecuteSolution(solution, descriptor, input))
                    PrintResult(result);
            }
            catch (Exception ex)
            {
                // If input fails (missing or empty), both parts are considered failed
                foreach (ushort part in new[] { 1, 2 })
                {
                    PrintResult(new ExecutionResult(
                        descriptor.Year,
                        descriptor.Day,
                        part,
                        Success: false,
                        Output: null,
                        Exception: ex
                    ));
                }
            }
        }
    }

    /// <summary>
    /// Instantiates all ISolution implementations described by the given descriptors.
    /// Any instantiation failures are aggregated and result in a single exception.
    /// </summary>
    private static ImmutableList<ISolution> LoadSolutions(IEnumerable<SolutionDescriptor> descriptors)
    {
        var solutions = ImmutableList.CreateBuilder<ISolution>();
        var failures = new List<(SolutionDescriptor Descriptor, Exception Exception)>();

        foreach (var descriptor in descriptors)
        {
            try
            {
                var instance = (ISolution?)Activator.CreateInstance(descriptor.Type);

                if (instance is null)
                {
                    failures.Add((descriptor,
                        new InvalidOperationException(
                            $"Activator returned null for type {descriptor.Type.FullName}"
                        )));
                    continue;
                }

                solutions.Add(instance);
            }
            catch (Exception ex)
            {
                // Capture any exception during instantiation
                failures.Add((descriptor, ex));
            }
        }

        // If no failures occurred, return all successful instances
        if (failures.Count == 0)
            return solutions.ToImmutable();

        // Build error message summarizing all failures
        var message =
            "Failed to instantiate one or more ISolution instances:\n" +
            string.Join("\n", failures.Select(f =>
                $"- Year {f.Descriptor.Year}, Day {f.Descriptor.Day}, Type {f.Descriptor.Type.FullName}\n" +
                $"  Exception: {f.Exception.GetType().Name}: {f.Exception.Message}"
            ));

        throw new InvalidOperationException(message);
    }

    /// <summary>
    /// Loads the embedded input.txt file associated with a solution.
    /// Throws if the file does not exist or is empty.
    /// </summary>
    private static string LoadInput(ushort year, ushort day)
    {
        var resourceName = $"AdventOfCode.Solutions.Year{year}.Day{day:D2}.input.txt";

        using var stream = SolutionAssembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new InvalidOperationException(
                $"Missing embedded input file: {resourceName}."
            );
        }

        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        // Require non-empty input content
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException(
                $"Embedded input file '{resourceName}' is empty."
            );
        }

        return content;
    }

    /// <summary>
    /// Executes both parts of a solution and yields their results.
    /// </summary>
    private static IEnumerable<ExecutionResult> ExecuteSolution(
        ISolution solution,
        SolutionDescriptor descriptor,
        string input)
    {
        yield return ExecutePart(1, descriptor, () => solution.SolvePartOne(input));
        yield return ExecutePart(2, descriptor, () => solution.SolvePartTwo(input));
    }

    /// <summary>
    /// Executes a single part of a solution (Part 1 or Part 2),
    /// capturing success or exception information.
    /// </summary>
    private static ExecutionResult ExecutePart(
        ushort part,
        SolutionDescriptor descriptor,
        Func<string> execute)
    {
        try
        {
            var output = execute();
            return new ExecutionResult(
                descriptor.Year,
                descriptor.Day,
                part,
                Success: true,
                Output: output,
                Exception: null
            );
        }
        catch (Exception ex)
        {
            return new ExecutionResult(
                descriptor.Year,
                descriptor.Day,
                part,
                Success: false,
                Output: null,
                Exception: ex
            );
        }
    }

    /// <summary>
    /// Prints a formatted success or failure message for a solution part.
    /// </summary>
    private static void PrintResult(ExecutionResult result)
    {
        if (result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK]   Year {result.Year} Day {result.Day:D2} Part {result.Part}:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"      {result.Output}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[FAIL] Year {result.Year} Day {result.Day:D2} Part {result.Part}:");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"      {result.Exception?.GetType().Name}: {result.Exception?.Message}");
        }

        Console.ResetColor();
    }
}