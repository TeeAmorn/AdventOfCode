using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day11;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var graph = ParseGraph(input);
        return CountPaths("you", graph).ToString();
    }

    public string SolvePartTwo(string input)
    {
        var graph = ParseGraph(input);
        return CountPathsWithRequirements("svr", graph).ToString();
    }

    private static Dictionary<string, HashSet<string>> ParseGraph(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var graph = new Dictionary<string, HashSet<string>>();

        foreach (var line in lines)
        {
            var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var source = parts[0].TrimEnd(':');
            var destinations = parts[1..];
            graph[source] = new HashSet<string>(destinations);
        }

        return graph;
    }

    private static ulong CountPaths(
        string node,
        Dictionary<string, HashSet<string>> graph,
        Dictionary<string, ulong>? memo = null
    )
    {
        memo ??= new Dictionary<string, ulong>();

        if (node == "out")
            return 1;

        if (memo.TryGetValue(node, out var cached))
            return cached;

        if (!graph.TryGetValue(node, out var neighbors))
            return 0;

        var total = neighbors
            .Select(neighbor => CountPaths(neighbor, graph, memo))
            .Aggregate((a, b) => a + b);
        memo[node] = total;
        return total;
    }

    private static ulong CountPathsWithRequirements(
        string node,
        Dictionary<string, HashSet<string>> graph,
        Dictionary<(string, bool, bool), ulong>? memo = null,
        bool visitedFft = false,
        bool visitedDac = false
    )
    {
        memo ??= new Dictionary<(string, bool, bool), ulong>();

        if (node == "out")
            return visitedFft && visitedDac ? 1UL : 0UL;

        var state = (node, visitedFft, visitedDac);
        if (memo.TryGetValue(state, out var cached))
            return cached;

        if (!graph.TryGetValue(node, out var neighbors))
            return 0;

        var newVisitedFft = visitedFft || node == "fft";
        var newVisitedDac = visitedDac || node == "dac";

        var total = neighbors
            .Select(neighbor =>
                CountPathsWithRequirements(neighbor, graph, memo, newVisitedFft, newVisitedDac)
            )
            .Aggregate((a, b) => a + b);

        memo[state] = total;
        return total;
    }
}
