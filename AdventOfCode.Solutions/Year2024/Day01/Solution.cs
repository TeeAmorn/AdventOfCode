using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day01;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        (List<int> left, List<int> right) = ([], []);

        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(values[0]));
            right.Add(int.Parse(values[1]));
        }

        left.Sort();
        right.Sort();

        return left
            .Zip(right, (l, r) => Math.Abs(l - r))
            .Sum()
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        List<int> left = [];
        Dictionary<int, uint> right = new();

        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(values[0]));

            var rightValue = int.Parse(values[1]);
            if (!right.TryAdd(rightValue, 1))
                right[rightValue]++;
        }

        return left
            .Select(l => l * (right.TryGetValue(l, out var value) ? value : 0))
            .Sum()
            .ToString();
    }
}