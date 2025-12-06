using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day04;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var grid = ParseGrid(input);
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        return (from i in Enumerable.Range(0, rows)
                from j in Enumerable.Range(0, cols)
                select CountCrosswordsFrom(j, i, grid))
            .Sum()
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        var grid = ParseGrid(input);
        var rows = grid.GetLength(0);
        var cols = grid.GetLength(1);

        return (from i in Enumerable.Range(0, rows)
                from j in Enumerable.Range(0, cols)
                where FoundXWord(i, j, grid)
                select 1)
            .Sum()
            .ToString();
    }

    private static int[,] ParseGrid(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var rows = lines.Length;
        var cols = lines[0].Length;
        var grid = new int[rows, cols];
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                grid[row, col] = lines[row][col] switch
                {
                    'X' => 1,
                    'M' => 2,
                    'A' => 3,
                    'S' => 4,
                    _ => 0,
                };
            }
        }

        return grid;
    }

    private static int CountCrosswordsFrom(int i, int j, int[,] grid)
    {
        // Only start searching if we're at an 'X' (value 1)
        if (grid[i, j] != 1)
            return 0;

        (int di, int dj)[] deltas =
        [
            (-1, -1),
            (-1, 0),
            (-1, 1),
            (0, -1),
            (0, 1),
            (1, -1),
            (1, 0),
            (1, 1)
        ];

        return (from d in deltas
                where SearchDirection(i, j, d.di, d.dj, grid)
                select 1)
            .Sum();
    }

    private static bool SearchDirection(int i, int j, int di, int dj, int[,] grid)
    {
        var val = 1;
        while (true)
        {
            if (grid[i, j] != val)
                return false;

            if (grid[i, j] == 4)
                return true;

            var ni = i + di;
            var nj = j + dj;
            if (ni < 0 || ni >= grid.GetLength(0) || nj < 0 || nj >= grid.GetLength(1))
                return false;

            i = ni;
            j = nj;
            val += 1;
        }
    }

    private static bool FoundXWord(int i, int j, int[,] grid)
    {
        if (i == 0 || i >= (grid.GetLength(0) - 1) || j == 0 || j >= (grid.GetLength(1) - 1))
            return false;

        if (grid[i, j] != 3)
            return false;

        var leftDiagonal = new[] { grid[i - 1, j - 1], grid[i + 1, j + 1] };
        var rightDiagonal = new[] { grid[i - 1, j + 1], grid[i + 1, j - 1] };
        return leftDiagonal.Contains(2) &&
               leftDiagonal.Contains(4) &&
               rightDiagonal.Contains(2) &&
               rightDiagonal.Contains(4);
    }
}