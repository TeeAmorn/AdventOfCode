using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2024.Day09;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var blocks = ParseInput(input);
        var leftNode = blocks.First;
        var rightNode = blocks.Last;

        while (leftNode != rightNode)
        {
            if (leftNode == null || rightNode == null)
                throw new UnreachableException("Linked list node became null unexpectedly.");

            var leftBlock = leftNode.Value;
            var rightBlock = rightNode.Value;

            // Skip file blocks on the left since we only want to fill space blocks
            if (leftBlock.Index != null)
            {
                leftNode = leftNode.Next;
                continue;
            }

            // Remove trailing space blocks on the right
            if (rightBlock.Index == null)
            {
                var nodeToRemove = rightNode;
                rightNode = rightNode.Previous;
                blocks.Remove(nodeToRemove);
                continue;
            }

            // Move file blocks from right into space blocks on left
            if (leftBlock.Size <= rightBlock.Size)
            {
                // Space block is smaller or equal - fill it completely with file data
                leftBlock.Index = rightBlock.Index;
                rightBlock.Size -= leftBlock.Size;

                // If right file block is fully consumed, remove it
                if (rightBlock.Size != 0)
                    continue;
                var nodeToRemove = rightNode;
                rightNode = rightNode.Previous;
                blocks.Remove(nodeToRemove);
            }
            else
            {
                // Space block is larger - move entire file block into it
                var fileNode = rightNode;
                rightNode = rightNode.Previous;
                blocks.Remove(fileNode);

                leftBlock.Size -= rightBlock.Size;
                blocks.AddBefore(leftNode, fileNode);
            }
        }

        return CalculateChecksum(blocks).ToString();
    }

    public string SolvePartTwo(string input)
    {
        var blocks = ParseInput(input);
        var currentFile = blocks.Last;

        // Process each file block from right to left
        while (currentFile != null)
        {
            // Skip space blocks
            if (currentFile.Value.Index == null)
            {
                currentFile = currentFile.Previous;
                continue;
            }

            // Find the leftmost space block that can fit this file
            var candidateSpace = blocks.First;
            while (candidateSpace != currentFile)
            {
                if (candidateSpace == null)
                    throw new UnreachableException("Linked list node became null unexpectedly.");

                var spaceBlock = candidateSpace.Value;
                var fileBlock = currentFile.Value;

                // Skip file blocks and spaces that are too small
                if (spaceBlock.Index != null || spaceBlock.Size < fileBlock.Size)
                {
                    candidateSpace = candidateSpace.Next;
                    continue;
                }

                // Found a suitable space - move the file
                if (spaceBlock.Size == fileBlock.Size)
                {
                    // Exact fit - swap the blocks
                    (currentFile.Value, candidateSpace.Value) = (
                        candidateSpace.Value,
                        currentFile.Value
                    );
                }
                else
                {
                    // Space is larger - insert file and shrink the space
                    var movedFile = fileBlock with
                    { };
                    blocks.AddBefore(candidateSpace, movedFile);
                    spaceBlock.Size -= fileBlock.Size;

                    // Convert original file location to space
                    fileBlock.Index = null;
                }

                break;
            }

            currentFile = currentFile.Previous;
        }

        return CalculateChecksum(blocks).ToString();
    }

    private record Block
    {
        public required ulong Size { get; set; }
        public ulong? Index { get; set; }
    }

    private static LinkedList<Block> ParseInput(string input)
    {
        LinkedList<Block> blocks = [];

        for (var i = 0; i < input.Length; i++)
        {
            var size = ulong.Parse(input[i].ToString());

            // Skip zero-size blocks
            if (size == 0)
                continue;

            // Even positions are files, odd positions are spaces
            var block =
                i % 2 == 0
                    ? new Block { Index = Convert.ToUInt64(i / 2), Size = size }
                    : new Block { Index = null, Size = size };

            blocks.AddLast(block);
        }

        return blocks;
    }

    private static ulong CalculateChecksum(LinkedList<Block> blocks)
    {
        var checksum = 0UL;
        var position = 0UL;

        foreach (var block in blocks)
        {
            // Only file blocks contribute to checksum
            if (block.Index != null)
            {
                // Sum of arithmetic sequence: fileId * (first + last) * count / 2
                // where first = position, last = position + size - 1
                checksum += block.Index.Value * block.Size * (2 * position + block.Size - 1) / 2;
            }

            position += block.Size;
        }

        return checksum;
    }
}
