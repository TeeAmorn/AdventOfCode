using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day04;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var grid = ParseGrid(input);
        return RemoveIsolatedCells(grid).ToString();
    }

    public string SolvePartTwo(string input)
    {
        var grid = ParseGrid(input);
        var totalRemoved = 0;
        int removedThisIteration;

        do
        {
            removedThisIteration = RemoveIsolatedCells(grid);
            totalRemoved += removedThisIteration;
        } while (removedThisIteration > 0);

        return totalRemoved.ToString();
    }

    private static int[,] ParseGrid(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var rows = lines.Length;
        var cols = lines[0].Length;

        // Add border padding to avoid boundary checks during neighbor counting
        var grid = new int[rows + 2, cols + 2];

        // Populate grid with 1 for occupied cells ('@'), 0 for empty
        // Offset by 1 to account for border padding
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                grid[row + 1, col + 1] = lines[row][col] == '@' ? 1 : 0;
            }
        }

        return grid;
    }

    private static int RemoveIsolatedCells(int[,] grid)
    {
        var cellsToRemove = new List<(int row, int col)>();
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        // Find all occupied cells with fewer than 4 neighbors
        // Skip border (index 0 and last index) since it's just padding
        for (var row = 1; row < rows - 1; row++)
        {
            for (var col = 1; col < cols - 1; col++)
            {
                if (grid[row, col] == 0)
                    continue;

                var neighborCount = CountOccupiedNeighbors(grid, row, col);

                if (neighborCount < 4)
                    cellsToRemove.Add((row, col));
            }
        }

        // Remove all isolated cells from the grid
        foreach (var (row, col) in cellsToRemove)
            grid[row, col] = 0;

        return cellsToRemove.Count;
    }

    private static int CountOccupiedNeighbors(int[,] grid, int row, int col)
    {
        // Count all 8 surrounding cells in a 3x3 grid around the target cell
        return grid[row - 1, col - 1]
            + grid[row - 1, col]
            + grid[row - 1, col + 1]
            + grid[row, col - 1]
            + grid[row, col + 1]
            + grid[row + 1, col - 1]
            + grid[row + 1, col]
            + grid[row + 1, col + 1];
    }
}
