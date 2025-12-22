using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day11;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input) =>
        CountStonesAfterBlinks(input, blinks: 25).ToString();

    public string SolvePartTwo(string input) =>
        CountStonesAfterBlinks(input, blinks: 75).ToString();

    private static ulong CountStonesAfterBlinks(string input, int blinks)
    {
        var cache = new Dictionary<(ulong stone, int blinksRemaining), ulong>();

        return input
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(ulong.Parse)
            .Select(stone => CountStonesFromSingle(stone, blinks, cache))
            .Aggregate((a, b) => a + b);
    }

    private static ulong CountStonesFromSingle(
        ulong stone,
        int blinksRemaining,
        Dictionary<(ulong, int), ulong> cache
    )
    {
        if (cache.TryGetValue((stone, blinksRemaining), out var cachedResult))
            return cachedResult;

        if (blinksRemaining == 0)
            return 1;

        var stoneString = stone.ToString();
        ulong[] nextStones = stoneString switch
        {
            "0" => [1UL],
            { Length: var len } when len % 2 == 0 =>
            [
                ulong.Parse(stoneString[..(len / 2)]),
                ulong.Parse(stoneString[(len / 2)..]),
            ],
            _ => [stone * 2024],
        };

        var result = nextStones
            .Select(s => CountStonesFromSingle(s, blinksRemaining - 1, cache))
            .Aggregate((a, b) => a + b);
        cache[(stone, blinksRemaining)] = result;
        return result;
    }
}
