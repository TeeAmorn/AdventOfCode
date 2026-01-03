using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AdventOfCode.Solutions.Year2024.Day12;

using Region = HashSet<Complex>;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    private static readonly Complex North = Complex.ImaginaryOne;
    private static readonly Complex South = -Complex.ImaginaryOne;
    private static readonly Complex West = -1;
    private static readonly Complex East = 1;

    private static readonly Complex[] CardinalDirections = [North, South, West, East];

    public string SolvePartOne(string input)
    {
        var regionsByPosition = BuildRegions(input);
        return regionsByPosition
            .Values.Distinct()
            .Select(region =>
                region.Sum(position => GetPerimeterEdges(position, regionsByPosition))
                * region.Count
            )
            .Sum()
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        var regionsByPosition = BuildRegions(input);
        return regionsByPosition
            .Values.Distinct()
            .Select(region =>
                region.Sum(position => GetCorners(position, regionsByPosition)) * region.Count
            )
            .Sum()
            .ToString();
    }

    private static Dictionary<Complex, Region> BuildRegions(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var grid = (
            from y in Enumerable.Range(0, lines.Length)
            from x in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Complex, char>(x + y * South, lines[y][x])
        ).ToDictionary();

        var regionsByPosition = new Dictionary<Complex, Region>();
        var unvisited = grid.Keys.ToHashSet();

        while (unvisited.Count > 0)
        {
            var startPosition = unvisited.First();
            var region = new Region();
            var queue = new Queue<Complex>();
            queue.Enqueue(startPosition);

            var plantType = grid[startPosition];

            while (queue.Count > 0)
            {
                var position = queue.Dequeue();

                if (!unvisited.Contains(position))
                    continue;

                region.Add(position);
                regionsByPosition[position] = region;
                unvisited.Remove(position);

                foreach (var direction in CardinalDirections)
                {
                    var neighbor = position + direction;
                    if (
                        unvisited.Contains(neighbor)
                        && grid.GetValueOrDefault(neighbor) == plantType
                    )
                        queue.Enqueue(neighbor);
                }
            }
        }

        return regionsByPosition;
    }

    private static int GetPerimeterEdges(
        Complex position,
        Dictionary<Complex, Region> regionsByPosition
    )
    {
        var currentRegion = regionsByPosition[position];
        return CardinalDirections.Count(direction =>
            regionsByPosition.GetValueOrDefault(position + direction) != currentRegion
        );
    }

    private static int GetCorners(Complex position, Dictionary<Complex, Region> regionsByPosition)
    {
        var corners = 0;
        var currentRegion = regionsByPosition[position];

        // Check all four potential corner configurations
        foreach (
            var (first, second) in new[]
            {
                (North, East),
                (East, South),
                (South, West),
                (West, North),
            }
        )
        {
            var firstInRegion =
                regionsByPosition.GetValueOrDefault(position + first) == currentRegion;
            var secondInRegion =
                regionsByPosition.GetValueOrDefault(position + second) == currentRegion;
            var diagonalInRegion =
                regionsByPosition.GetValueOrDefault(position + first + second) == currentRegion;

            switch (firstInRegion)
            {
                // Convex corner: both adjacent sides outside region
                case false when !secondInRegion:
                // Concave corner: both adjacent sides in region, diagonal outside
                case true when secondInRegion && !diagonalInRegion:
                    corners++;
                    break;
            }
        }

        return corners;
    }
}
