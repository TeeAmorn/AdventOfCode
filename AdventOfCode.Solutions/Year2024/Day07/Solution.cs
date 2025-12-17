using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day07;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input) =>
        (
            from eq in ParseEquations(input)
            where CanSolve(eq.target, eq.operands, false)
            select eq.target
        )
            .Aggregate(0UL, (sum, val) => sum + val)
            .ToString();

    public string SolvePartTwo(string input)
    {
        var results = ParseEquations(input)
            .Select(eq => (isValid: CanSolve(eq.target, eq.operands, false), equation: eq))
            .ToList();

        var sum = results
            .Where(result => result.isValid)
            .Select(result => result.equation)
            .Aggregate(0UL, (acc, eq) => acc + eq.target);

        return results
            .Where(result => !result.isValid)
            .Select(result => result.equation)
            .Where(eq => CanSolve(eq.target, eq.operands, true))
            .Aggregate(sum, (acc, eq) => acc + eq.target)
            .ToString();
    }

    private static IEnumerable<(ulong target, ulong[] operands)> ParseEquations(string input) =>
        from line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
        let parts = line.Split(':')
        select (
            target: ulong.Parse(parts[0]),
            operands: parts[1]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(ulong.Parse)
                .ToArray()
        );

    private static bool CanSolve(
        ulong target,
        ulong[] operands,
        bool allowConcat,
        int index = 0,
        ulong current = 0
    )
    {
        if (index == operands.Length)
            return current == target;

        if (current > target)
            return false;

        var next = operands[index];
        return CanSolve(target, operands, allowConcat, index + 1, current + next)
            || CanSolve(target, operands, allowConcat, index + 1, current * next)
            || (
                allowConcat
                && CanSolve(
                    target,
                    operands,
                    allowConcat,
                    index + 1,
                    ulong.Parse($"{current}{next}")
                )
            );
    }
}
