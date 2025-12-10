using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day10;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var machines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        var totalPresses = machines
            .Select(ParseMachine)
            .Select(machine =>
                FindMinimumButtonPresses(machine.TargetLights, machine.ButtonConfigurations)
            )
            .Sum(presses => presses.Count);

        return totalPresses.ToString();
    }

    public string SolvePartTwo(string input)
    {
        throw new NotImplementedException();
    }

    private static MachineConfiguration ParseMachine(string machineSpec)
    {
        var components = machineSpec
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part[1..^1]) // Remove brackets
            .ToArray();

        var targetLights = components[0].Select(c => c == '#').ToArray();

        var numericComponents = components[1..]
            .Select(part =>
                part.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray()
            )
            .ToArray();

        var buttonConfigurations = numericComponents[..^1];
        var joltageRequirements = numericComponents[^1];

        return new MachineConfiguration(targetLights, buttonConfigurations, joltageRequirements);
    }

    private static List<int> FindMinimumButtonPresses(
        bool[] targetLights,
        int[][] buttonConfigurations
    )
    {
        var initialState = new LightState(new bool[targetLights.Length], new List<int>());

        var queue = new Queue<LightState>();
        queue.Enqueue(initialState);

        while (queue.Count > 0)
        {
            var currentState = queue.Dequeue();

            for (var buttonIndex = 0; buttonIndex < buttonConfigurations.Length; buttonIndex++)
            {
                var newLights = (bool[])currentState.Lights.Clone();

                foreach (var lightIndex in buttonConfigurations[buttonIndex])
                    newLights[lightIndex] = !newLights[lightIndex];

                var newPressedButtons = new List<int>(currentState.PressedButtons) { buttonIndex };

                var newState = new LightState(newLights, newPressedButtons);

                if (newState.Lights.Zip(targetLights).All(pair => pair.First == pair.Second))
                    return newState.PressedButtons;

                queue.Enqueue(newState);
            }
        }

        throw new Exception("No solutions found");
    }

    private record MachineConfiguration(
        bool[] TargetLights,
        int[][] ButtonConfigurations,
        int[] JoltageRequirements
    );

    private record LightState(bool[] Lights, List<int> PressedButtons);
}