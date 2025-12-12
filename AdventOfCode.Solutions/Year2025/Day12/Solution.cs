using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day12;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var sections = input.Split(
            $"{Environment.NewLine}{Environment.NewLine}",
            StringSplitOptions.RemoveEmptyEntries
        );

        var sizes = sections[..^1].Select(section => section.Count(c => c == '#')).ToList();

        var regions = sections[^1]
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(region =>
            {
                var split = region.Split(" ");
                var dimensions = split[0][..^1].Split('x').Select(int.Parse).ToList();
                var requirements = split.Skip(1).Select(int.Parse).ToList();
                return new Region(dimensions[0], dimensions[1], requirements);
            });

        return regions
            .Where(region =>
            {
                var spaceNeeded = region.Requirements.Select((n, i) => n * sizes[i]).Sum();
                var spaceAvailable = region.X * region.Y;
                return spaceAvailable >= spaceNeeded;
            })
            .Count()
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        return "No part 2!";
    }
}

public record struct Region(int X, int Y, List<int> Requirements);
