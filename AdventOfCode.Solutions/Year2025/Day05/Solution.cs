using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day05;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var (rangeSection, querySection) = ParseInputSections(input);
        var mergedRanges = MergeOverlappingRanges(rangeSection);

        return querySection
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(ulong.Parse)
            .Count(id => mergedRanges.Any(range => range.Contains(id)))
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        var (rangeSection, _) = ParseInputSections(input);
        var mergedRanges = MergeOverlappingRanges(rangeSection);

        return mergedRanges.Aggregate(0UL, (acc, range) => acc + range.Length).ToString();
    }

    private static (string ranges, string queries) ParseInputSections(string input) =>
        input.Split(
            $"{Environment.NewLine}{Environment.NewLine}",
            StringSplitOptions.RemoveEmptyEntries
        ) switch
        {
            var s => (s[0], s[1]),
        };

    private static IReadOnlyList<Range> MergeOverlappingRanges(string rangeSection)
    {
        var ranges = rangeSection
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split('-');
                return new Range(ulong.Parse(parts[0]), ulong.Parse(parts[1]));
            })
            .OrderBy(r => r.Start)
            .ThenBy(r => r.End)
            .ToArray();

        if (ranges.Length == 0)
            return [];

        var merged = new List<Range> { ranges[0] };

        foreach (var current in ranges.Skip(1))
        {
            var previous = merged[^1];

            if (current.OverlapsWith(previous))
                merged[^1] = previous.MergeWith(current);
            else
                merged.Add(current);
        }

        return merged;
    }

    private readonly record struct Range(ulong Start, ulong End)
    {
        public ulong Length => End - Start + 1;

        public bool Contains(ulong value) => value >= Start && value <= End;

        public bool OverlapsWith(Range other) => Start <= other.End + 1;

        public Range MergeWith(Range other) => new(Start: Start, End: Math.Max(End, other.End));
    }
}
