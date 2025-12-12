using System.Diagnostics.CodeAnalysis;
using Google.OrTools.LinearSolver;

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
        var machines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        return machines
            .Select(ParseMachine)
            .Select(machine =>
            {
                var lengthM = machine.TargetLights.Length;
                var lengthN = machine.ButtonConfigurations.Length;

                // Construct matrix A
                var matrixA = new int[lengthM, lengthN];
                for (var i = 0; i < lengthM; i++)
                for (var j = 0; j < lengthN; j++)
                    if (machine.ButtonConfigurations[j].Contains(i))
                        matrixA[i, j] = 1;

                // Construct vector b
                var vectorB = new int[lengthM];
                for (var i = 0; i < lengthM; i++)
                    vectorB[i] = machine.JoltageRequirements[i];

                // Initialize solver
                var solver = Solver.CreateSolver("SCIP");
                if (solver == null)
                    throw new Exception("Could not create solver GLOP");

                // Create variables x[0], x[1], ..., x[N-1] with non-negativity constraints
                var x = new Variable[lengthN];
                for (var j = 0; j < lengthN; j++)
                    x[j] = solver.MakeIntVar(0.0, double.PositiveInfinity, $"x_{j}");

                // Add constraints: Ax = b
                for (var i = 0; i < lengthM; i++)
                {
                    var constraint = solver.MakeConstraint(
                        vectorB[i],
                        vectorB[i],
                        $"constraint_{i}"
                    );
                    for (var j = 0; j < lengthN; j++)
                        constraint.SetCoefficient(x[j], matrixA[i, j]);
                }

                // Objective: minimize sum of x
                var objective = solver.Objective();
                for (var j = 0; j < lengthN; j++)
                    objective.SetCoefficient(x[j], 1.0);
                objective.SetMinimization();

                // Solve linear equations
                var resultStatus = solver.Solve();
                if (resultStatus != Solver.ResultStatus.OPTIMAL)
                    throw new Exception(
                        $"The problem does not have an optimal solution. Status: {resultStatus}"
                    );
                var result = new double[lengthN];
                for (var j = 0; j < lengthN; j++)
                    result[j] = Math.Round(x[j].SolutionValue());

                return Convert.ToInt32(result.Aggregate((acc, n) => acc + n));
            })
            .Sum()
            .ToString();
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
        var initialState = new LightState(new bool[targetLights.Length], []);

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
