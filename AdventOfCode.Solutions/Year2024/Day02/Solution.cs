using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day02;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input) =>
        ParseReports(input)
            .Count(IsValid)
            .ToString();

    public string SolvePartTwo(string input) =>
        ParseReports(input)
            .Count(report => GenerateReportVariations(report).Any(IsValid))
            .ToString();

    private static IEnumerable<int[]> ParseReports(string input) =>
        input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
                line
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray());

    private static IEnumerable<int[]> GenerateReportVariations(int[] report) =>
        Enumerable
            .Range(0, report.Length)
            .Select(i =>
                report
                    .Take(i)
                    .Concat(report.Skip(i + 1))
                    .ToArray());

    private static bool IsValid(int[] report)
    {
        var pairs = report.Zip(report.Skip(1)).ToList();
        return pairs.All(p => p.First - p.Second >= 1 && p.First - p.Second <= 3) ||
               pairs.All(p => p.Second - p.First >= 1 && p.Second - p.First <= 3);
    }
}