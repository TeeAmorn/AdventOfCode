using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day07;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var (manifold, beamStartIndex) = ParseInput(input);
        HashSet<int> beams = [beamStartIndex];

        var result = 0;
        foreach (var splitters in manifold)
        {
            var activeSplitters = beams.Intersect(splitters).ToList();
            result += activeSplitters.Count();

            foreach (var splitter in activeSplitters)
            {
                beams.Remove(splitter);
                beams.Add(splitter - 1);
                beams.Add(splitter + 1);
            }
        }

        return result.ToString();
    }

    public string SolvePartTwo(string input)
    {
        var (manifold, beamStartIndex) = ParseInput(input);
        return CountPath(beamStartIndex, 0, manifold.ToList(), new Dictionary<(int, int), ulong>())
            .ToString();
    }

    private static (IEnumerable<HashSet<int>> manifold, int beamStartIndex) ParseInput(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var manifold = lines
            .Where(line => line.Contains("^"))
            .Select(line => AllIndexesOf(line, "^").ToHashSet());
        var beamStartIndex = lines[0].IndexOf("S", StringComparison.Ordinal);
        return (manifold, beamStartIndex);
    }

    private static IEnumerable<int> AllIndexesOf(string str, string value)
    {
        for (var index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index, StringComparison.Ordinal);
            if (index == -1)
                break;
            yield return index;
        }
    }

    private static ulong CountPath(
        int beam,
        int level,
        List<HashSet<int>> manifold,
        Dictionary<(int, int), ulong> seen
    )
    {
        if (level == manifold.Count)
            return 1UL;

        if (seen.ContainsKey((beam, level)))
            return seen[(beam, level)];

        if (manifold[level].Contains(beam))
        {
            var result =
                CountPath(beam - 1, level + 1, manifold, seen)
                + CountPath(beam + 1, level + 1, manifold, seen);
            seen[(beam, level)] = result;
            return result;
        }
        else
        {
            var result = CountPath(beam, level + 1, manifold, seen);
            seen[(beam, level)] = result;
            return result;
        }
    }
}
