using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace AdventOfCode.Solutions.Year2024.Day06;

using Map = ImmutableDictionary<Complex, char>;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    private static readonly Complex Up = Complex.ImaginaryOne;
    private static readonly Complex TurnRight = -Complex.ImaginaryOne;
    private const char Obstacle = '#';
    private const char Guard = '^';

    public string SolvePartOne(string input)
    {
        var (map, start) = ParseInput(input);
        var (visitedPositions, _) = SimulateGuardPath(map, start);
        return visitedPositions.Count().ToString();
    }

    public string SolvePartTwo(string input)
    {
        var (map, start) = ParseInput(input);
        var (visitedPositions, _) = SimulateGuardPath(map, start);

        // Count positions where placing an obstacle creates a loop
        return visitedPositions
            .AsParallel()
            .Count(pos => SimulateGuardPath(map.SetItem(pos, Obstacle), start).isLoop)
            .ToString();
    }

    private static (IEnumerable<Complex> visitedPositions, bool isLoop) SimulateGuardPath(
        Map map,
        Complex position
    )
    {
        var visited = new HashSet<(Complex position, Complex direction)>();
        var direction = Up;

        while (map.ContainsKey(position) && visited.Add((position, direction)))
        {
            var nextPosition = position + direction;

            if (map.GetValueOrDefault(nextPosition) == Obstacle)
                direction *= TurnRight;
            else
                position = nextPosition;
        }

        var isLoop = map.ContainsKey(position); // Still on map means we hit a cycle
        var uniquePositions = visited.Select(state => state.position).Distinct();

        return (uniquePositions, isLoop);
    }

    private static (Map map, Complex start) ParseInput(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var map = (
            from y in Enumerable.Range(0, lines.Length)
            from x in Enumerable.Range(0, lines[y].Length)
            select new KeyValuePair<Complex, char>(x + Up * -y, lines[y][x])
        ).ToImmutableDictionary();

        var start = map.First(cell => cell.Value == Guard).Key;

        return (map, start);
    }
}
