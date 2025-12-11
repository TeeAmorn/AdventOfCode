using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day03;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input) => Solve(input, 2);

    public string SolvePartTwo(string input) => Solve(input, 12);

    private static string Solve(string input, int outputLength) =>
        input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(b => ExtractMaxDigitsSequentially(b, outputLength))
            .Select(ulong.Parse)
            .Aggregate((x, y) => x + y)
            .ToString();

    private static string ExtractMaxDigitsSequentially(string digitString, int outputLength)
    {
        // Convert string to array of digit values
        var digits = digitString.Select(c => (ulong)(c - '0')).ToArray();

        var selectedDigits = new List<ulong>(outputLength);
        var searchStart = 0;
        for (var position = 0; position < outputLength; position++)
        {
            // Calculate how many more digits we need after this one
            var remainingNeeded = outputLength - position;

            // Determine where we can search up to while leaving enough digits for future positions
            var searchEnd = digits.Length - remainingNeeded + 1;

            // Extract the valid search range for this position
            var searchRange = digits[searchStart..searchEnd];

            // Select the maximum digit from the valid range
            var maxDigit = searchRange.Max();
            selectedDigits.Add(maxDigit);

            // Move search start past the selected digit for the next iteration
            searchStart += searchRange.IndexOf(maxDigit) + 1;
        }

        // Convert selected digits back to a string representation
        return string.Join("", selectedDigits.Select(digit => digit.ToString()));
    }
}
