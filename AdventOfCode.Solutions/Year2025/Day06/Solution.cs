using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day06;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        // Parse all rows except the last into a 2D grid of numbers
        var numberGrid = lines[..^1]
            .Select(line =>
                line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToArray()
            )
            .ToArray();

        // Transpose the grid: convert rows to columns
        var columnArrays = Enumerable
            .Range(0, numberGrid[0].Length)
            .Select(colIndex => numberGrid.Select(row => row[colIndex]).ToArray())
            .ToArray();

        // The last line contains the operations to apply to each column
        var operators = lines[^1].Split(" ", StringSplitOptions.RemoveEmptyEntries);

        // Apply each operation to its corresponding column and sum the results
        return columnArrays.Zip(operators, ApplyOperation).Aggregate((x, y) => x + y).ToString();
    }

    public string SolvePartTwo(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        // Each line (except last) represents a column of digits
        var digitColumns = lines[..^1];

        // The last line is a pattern of operators and spaces indicating how to group digits
        var operatorPattern = lines[^1];

        // Parse the pattern, extract digit groups, apply operations, and sum
        return ParseOperatorPattern(operatorPattern)
            .Select(segment =>
            {
                // Extract consecutive digits from each column at the specified position
                // Each column contributes one digit to form a multi-digit number
                var numbers = Enumerable
                    .Range(0, segment.digitCount)
                    .Select(offset =>
                    {
                        // Read one digit from each row at position (startIndex + offset)
                        var digitsAcrossRows = digitColumns
                            .Select(column => column[segment.startIndex + offset])
                            .ToArray();
                        return ulong.Parse(digitsAcrossRows);
                    });

                return ApplyOperation(numbers, segment.operatorSymbol.ToString());
            })
            .Aggregate((x, y) => x + y)
            .ToString();
    }

    private static ulong ApplyOperation(IEnumerable<ulong> values, string operatorSymbol) =>
        operatorSymbol switch
        {
            "*" => values.Aggregate(1UL, (product, value) => product * value),
            "+" => values.Aggregate(0UL, (sum, value) => sum + value),
            _ => throw new ArgumentException($"Unknown operator: {operatorSymbol}"),
        };

    private static IEnumerable<(
        int startIndex,
        char operatorSymbol,
        int digitCount
    )> ParseOperatorPattern(string pattern)
    {
        // Find all positions where operators appear
        var operatorPositions = pattern
            .Select((ch, index) => (ch, index))
            .Where(pair => pair.ch is '*' or '+')
            .Select(pair => pair.index)
            .ToList();

        // For each operator, calculate how many digits follow it (until the next operator or end)
        for (var i = 0; i < operatorPositions.Count; i++)
        {
            var currentPosition = operatorPositions[i];
            var nextPosition =
                i < operatorPositions.Count - 1 ? operatorPositions[i + 1] : pattern.Length;
            var digitCount = nextPosition - currentPosition - 1;

            yield return (currentPosition, pattern[currentPosition], digitCount);
        }
    }
}
