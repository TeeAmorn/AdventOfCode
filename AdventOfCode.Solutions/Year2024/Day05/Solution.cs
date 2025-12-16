using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day05;

using OrderingRules = Dictionary<int, HashSet<int>>;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var (orderingRules, pageUpdates) = ParsePuzzleInput(input);

        return pageUpdates
            .Where(pages =>
            {
                for (var i = 1; i < pages.Count; i++)
                {
                    if (!orderingRules.TryGetValue(pages[i - 1], out var eligiblePages))
                        return false;

                    if (!eligiblePages.Contains(pages[i]))
                        return false;
                }

                return true;
            })
            .Select(eligiblePages => eligiblePages[eligiblePages.Count / 2])
            .Sum()
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        var (orderingRules, pageUpdates) = ParsePuzzleInput(input);

        var comparer = new PageComparer(orderingRules);

        return pageUpdates
            .Where(pages =>
            {
                // Check if it's NOT already in correct order
                for (var i = 1; i < pages.Count; i++)
                {
                    if (
                        !orderingRules.TryGetValue(pages[i - 1], out var eligiblePages)
                        || !eligiblePages.Contains(pages[i])
                    )
                        return true; // Incorrectly ordered
                }
                return false; // Already correct
            })
            .Select(pages =>
            {
                pages.Sort(comparer);
                return pages[pages.Count / 2];
            })
            .Sum()
            .ToString();
    }

    private static (OrderingRules OrderingRules, List<List<int>> PageUpdates) ParsePuzzleInput(
        string input
    )
    {
        var sections = input.Split(
            $"{Environment.NewLine}{Environment.NewLine}",
            StringSplitOptions.RemoveEmptyEntries
        );

        var orderingRules = ParseOrderingRules(sections[0]);

        var pageUpdates = sections[1]
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(updateLine =>
                updateLine
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList()
            )
            .ToList();

        return (orderingRules, pageUpdates);
    }

    private static OrderingRules ParseOrderingRules(string input) =>
        input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(ruleLine =>
            {
                var parts = ruleLine.Split("|", StringSplitOptions.RemoveEmptyEntries);
                return (Before: int.Parse(parts[0]), After: int.Parse(parts[1]));
            })
            .GroupBy(rule => rule.Before)
            .ToDictionary(
                rulesForPage => rulesForPage.Key,
                rulesForPage => rulesForPage.Select(rule => rule.After).ToHashSet()
            );

    private class PageComparer(OrderingRules orderingRules) : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            // If x must come before y (y is in x's "after" set), return -1
            if (orderingRules.TryGetValue(x, out var xAfter) && xAfter.Contains(y))
                return -1;

            // If y must come before x (x is in y's "after" set), return 1
            if (orderingRules.TryGetValue(y, out var yAfter) && yAfter.Contains(x))
                return 1;

            // No ordering rule between them
            return 0;
        }
    }
}
