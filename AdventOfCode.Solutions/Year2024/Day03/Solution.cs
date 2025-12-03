using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2024.Day03;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input) => Solve(input, @"mul\((\d+),(\d+)\)");

    public string SolvePartTwo(string input) => Solve(input, @"mul\((\d+),(\d+)\)|don't\(\)|do\(\)");

    private static string Solve(string input, string regex)
    {
        var matches = Regex.Matches(input, regex, RegexOptions.Multiline);
        return matches.Aggregate(
            (enabled: true, res: 0),
            (acc, m) =>
                (m.Value, acc.res, acc.enabled) switch
                {
                    ("don't()", _, _) => (false, acc.res),
                    ("do()", _, _) => (true, acc.res),
                    (_, var res, true) =>
                        (true, res + int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value)),
                    _ => acc
                },
            acc => acc.res.ToString()
        );
    }
}