using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;
using AdventOfCode.Solutions;

namespace AdventOfCode.Cli;

public readonly record struct SolutionDescriptor(ushort Year, ushort Day);

public static class Runner
{
    private static readonly Assembly SolutionAssembly = typeof(ISolution).Assembly;

    private static readonly Regex SolutionRegex =
        new(@"^AdventOfCode\.Solutions\.Year(?<year>\d{4})\.Day(?<day>\d{2})$");

    /// <summary>
    /// Gets a list of all available solutions in the assembly.
    /// This list is lazily evaluated and cached as an immutable list.
    /// </summary>
    public static ImmutableList<SolutionDescriptor> SolutionDescriptors
    {
        get
        {
            // Return cached value if already initialized
            if (field != null) return field;

            // Iterate through all types in the solutions assembly
            var builder = ImmutableList.CreateBuilder<SolutionDescriptor>();
            foreach (var type in SolutionAssembly.GetTypes())
            {
                // Only consider classes implementing ISolution
                if (!typeof(ISolution).IsAssignableFrom(type) || !type.IsClass)
                    continue;

                // Ensure the type has a namespace
                var ns = type.Namespace;
                if (ns == null)
                    continue;

                // Match the namespace to extract year and day
                var match = SolutionRegex.Match(ns);
                if (!match.Success)
                    continue;

                // Parse year and day from matched groups
                if (!ushort.TryParse(match.Groups["year"].Value, out var parsedYear))
                    continue;
                if (!ushort.TryParse(match.Groups["day"].Value, out var parsedDay))
                    continue;

                // Add a new descriptor for this solution
                builder.Add(new SolutionDescriptor(parsedYear, parsedDay));
            }

            // Finalize the list as immutable and cache it
            field = builder.ToImmutable();
            return field;
        }
    }

    /// <summary>
    /// Tries to run the solution for the given year and optional day.
    /// Returns true if the solutions ran successfully, false otherwise.
    /// </summary>
    /// <param name="year">The year of the solution to run.</param>
    /// <param name="day">The optional day of the solution to run.</param>
    /// <returns>True if execution succeeded, false otherwise.</returns>
    public static bool TryRun(ushort? year, ushort? day)
    {
        return false;
    }
}