using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AdventOfCode.Solutions.Year2024.Day08;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input) => CountAntinodes(input, getResonantAntinodes: false);

    public string SolvePartTwo(string input) => CountAntinodes(input, getResonantAntinodes: true);

    private static string CountAntinodes(string input, bool getResonantAntinodes)
    {
        var (antennaGroups, bounds) = ParseInput(input);

        var antinodes = (
            from antennas in antennaGroups
            from source in antennas
            from target in antennas
            where source != target
            from antinode in GenerateAntinodes(source, target, bounds, getResonantAntinodes)
            select antinode
        );

        return antinodes.ToHashSet().Count.ToString();
    }

    private static (IEnumerable<Complex[]> antennaGroups, (int rows, int cols) bounds) ParseInput(
        string input
    )
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var rows = lines.Length;
        var cols = lines[0].Length;

        var antennaGroups =
            from row in Enumerable.Range(0, rows)
            from col in Enumerable.Range(0, cols)
            let frequency = lines[row][col]
            where frequency != '.'
            let position = col + row * Complex.ImaginaryOne
            group position by frequency into frequencyGroup
            select frequencyGroup.ToArray();

        return (antennaGroups, (rows, cols));
    }

    private static IEnumerable<Complex> GenerateAntinodes(
        Complex source,
        Complex target,
        (int rows, int cols) bounds,
        bool resonant
    )
    {
        var direction = target - source;
        var position = resonant ? target : target + direction;

        while (
            position.Real >= 0
            && position.Real < bounds.cols
            && position.Imaginary >= 0
            && position.Imaginary < bounds.rows
        )
        {
            yield return position;
            if (!resonant)
                yield break;
            position += direction;
        }
    }
}
