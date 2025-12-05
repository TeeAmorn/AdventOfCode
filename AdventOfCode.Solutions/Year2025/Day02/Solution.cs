using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day02;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
        => GetAllIds(input)
            .Where(i => IsRepeatedText(i.ToString(), 2))
            .Aggregate(0UL, ((x, y) => x + y))
            .ToString();

    public string SolvePartTwo(string input)
        => (from id in GetAllIds(input)
                let idString = id.ToString()
                let isInvalid = Enumerable
                    .Range(1, idString.Length)
                    .Any(length => IsRepeatedText(idString, length))
                where isInvalid
                select id)
            .Aggregate(0UL, (current, id) => current + id).ToString();

    private static IEnumerable<ulong> GetAllIds(string input)
    {
        var ranges = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var range in ranges)
        {
            var r = range.Split('-');
            var min = ulong.Parse(r[0]);
            var max = ulong.Parse(r[1]);
            for (var i = min; i <= max; i++)
            {
                yield return i;
            }
        }
    }

    private static bool IsRepeatedText(string input, int n)
    {
        if (n == 1 || input.Length == 1 || input.Length % n != 0)
            return false;

        var segmentLength = input.Length / n;
        var segment = input[..segmentLength];
        for (var i = 1; i < n; i++)
        {
            if (segment != input.Substring(i * segmentLength, segmentLength))
                return false;
        }

        return true;
    }
}