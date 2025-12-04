using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day01;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var rotations = GetRotations(input);
        var position = 50;
        var count = 0;

        foreach (var rotation in rotations)
        {
            position = Mod(position + rotation, 100);
            if (position == 0) count++;
        }

        return count.ToString();
    }

    public string SolvePartTwo(string input)
    {
        var rotations = GetRotations(input);
        var position = 50;
        var count = 0;

        foreach (var rotation in rotations)
        {
            count += CountZeroCrossings(position, rotation);
            position = Mod(position + rotation, 100);
        }

        return count.ToString();
    }

    private static int CountZeroCrossings(int position, int rotation)
    {
        // Count complete revolutions
        var completeCycles = Math.Abs(rotation) / 100;

        // Check if partial rotation crosses zero
        var newPosition = position + (rotation % 100);
        var crossesZero = rotation > 0
            ? newPosition >= 100
            : position != 0 && newPosition <= 0;

        return completeCycles + (crossesZero ? 1 : 0);
    }

    private static int Mod(int value, int modulus)
    {
        var result = value % modulus;
        return result < 0 ? result + modulus : result;
    }

    private static IEnumerable<int> GetRotations(string input) =>
        input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => int.Parse(line[1..]) * (line[0] == 'L' ? -1 : 1));
}