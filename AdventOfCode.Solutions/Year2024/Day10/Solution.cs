using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day10;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
public class Solution : ISolution
{
    private static readonly (int dRow, int dCol)[] Directions = [(1, 0), (0, 1), (0, -1), (-1, 0)];

    public string SolvePartOne(string input)
    {
        var grid = ParseGrid(input);
        var reachablePeaks = new Dictionary<(int, int), HashSet<(int, int)>>();

        return GetTrailheads(grid)
            .Sum(trailhead =>
            {
                FindReachablePeaks(trailhead.row, trailhead.col, grid, reachablePeaks);
                return reachablePeaks[trailhead].Count;
            })
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        var grid = ParseGrid(input);
        var trailCounts = new Dictionary<(int, int), int>();

        return GetTrailheads(grid)
            .Sum(trailhead => CountUniqueTrails(trailhead.row, trailhead.col, grid, trailCounts))
            .ToString();
    }

    private static int[][] ParseGrid(string input) =>
        input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();

    private static IEnumerable<(int row, int col)> GetTrailheads(int[][] grid) =>
        from row in Enumerable.Range(0, grid.Length)
        from col in Enumerable.Range(0, grid[0].Length)
        where grid[row][col] == 0
        select (row, col);

    private static void FindReachablePeaks(
        int row,
        int col,
        int[][] grid,
        Dictionary<(int, int), HashSet<(int, int)>> reachablePeaks
    )
    {
        if (grid[row][col] == 9)
        {
            reachablePeaks[(row, col)] = [(row, col)];
            return;
        }

        if (reachablePeaks.ContainsKey((row, col)))
            return;

        reachablePeaks[(row, col)] = [];

        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = row + dRow;
            var nextCol = col + dCol;

            if (nextRow < 0 || nextRow >= grid.Length || nextCol < 0 || nextCol >= grid[0].Length)
                continue;

            if (grid[nextRow][nextCol] != grid[row][col] + 1)
                continue;

            FindReachablePeaks(nextRow, nextCol, grid, reachablePeaks);
            reachablePeaks[(row, col)].UnionWith(reachablePeaks[(nextRow, nextCol)]);
        }
    }

    private static int CountUniqueTrails(
        int row,
        int col,
        int[][] grid,
        Dictionary<(int, int), int> trailCounts
    )
    {
        if (grid[row][col] == 9)
            return 1;

        if (trailCounts.ContainsKey((row, col)))
            return trailCounts[(row, col)];

        trailCounts[(row, col)] = 0;

        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = row + dRow;
            var nextCol = col + dCol;

            if (nextRow < 0 || nextRow >= grid.Length || nextCol < 0 || nextCol >= grid[0].Length)
                continue;

            if (grid[nextRow][nextCol] != grid[row][col] + 1)
                continue;

            trailCounts[(row, col)] += CountUniqueTrails(nextRow, nextCol, grid, trailCounts);
        }

        return trailCounts[(row, col)];
    }
}
