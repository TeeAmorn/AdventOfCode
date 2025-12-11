using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day08;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var lines = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var nodes =
            from line in lines
            let coordinate = line.Split(',').Select(double.Parse).ToArray()
            select new Node(coordinate[0], coordinate[1], coordinate[2]);
        var graph = new SpatialGraph(nodes);

        var edges = graph.GetAllPossibleEdges().ToList();
        edges.Sort((edge1, edge2) => edge1.Weight.CompareTo(edge2.Weight));

        var edgesToAdd = lines.Length == 20 ? 10 : 1000;
        for (var i = 0; i < edgesToAdd; i++)
            graph.AddEdge(edges[i].N1, edges[i].N2);

        var islands = graph.GetIslands().Select(island => island.Count).ToList();
        islands.Sort();
        islands.Reverse();

        return islands.Take(3).Aggregate(1, (x, y) => x * y).ToString();
    }

    public string SolvePartTwo(string input)
    {
        var nodes =
            from line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            let coordinate = line.Split(',').Select(double.Parse).ToArray()
            select new Node(coordinate[0], coordinate[1], coordinate[2]);
        var graph = new SpatialGraph(nodes);

        var edges = graph.GetAllPossibleEdges().ToList();
        edges.Sort((edge1, edge2) => edge1.Weight.CompareTo(edge2.Weight));

        Edge edge;
        do
        {
            edge = edges[0];
            edges.RemoveAt(0);
            graph.AddEdge(edge.N1, edge.N2);
        } while (graph.GetIslandCount() > 1);

        return Convert.ToUInt64(edge.N1.X * edge.N2.X).ToString();
    }

    private static SpatialGraph ParseInput(string input)
    {
        var nodes =
            from line in input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            let coordinate = line.Split(',').Select(double.Parse).ToArray()
            select new Node(coordinate[0], coordinate[1], coordinate[2]);

        return new SpatialGraph(nodes);
    }
}

public record Node(double X, double Y, double Z);

public record Edge(Node N1, Node N2, double Weight);

public class SpatialGraph
{
    private readonly Node[] _nodes;
    private readonly bool[][] _edges;
    private readonly int _nodeCount;
    private readonly UnionFind _unionFind;

    public SpatialGraph(IEnumerable<Node> nodes)
    {
        _nodes = nodes.ToArray();
        _nodeCount = _nodes.Length;

        _edges = new bool[_nodeCount][];
        for (var i = 0; i < _nodeCount; i++)
            _edges[i] = new bool[_nodeCount - i - 1];

        _unionFind = new UnionFind(_nodeCount);
    }

    public IEnumerable<Edge> GetAllPossibleEdges()
    {
        for (var i = 0; i < _nodeCount; i++)
        for (var j = i + 1; j < _nodeCount; j++)
            yield return new Edge(_nodes[i], _nodes[j], CalculateDistance(_nodes[i], _nodes[j]));
    }

    public void AddEdge(Node node1, Node node2)
    {
        var idx1 = Array.IndexOf(_nodes, node1);
        var idx2 = Array.IndexOf(_nodes, node2);

        if (idx1 == -1 || idx2 == -1)
            throw new ArgumentException("Node not found in graph");

        if (idx1 == idx2)
            return; // No self-loops

        // Ensure idx1 < idx2 for upper triangle storage
        if (idx1 > idx2)
            (idx1, idx2) = (idx2, idx1);

        _edges[idx1][idx2 - idx1 - 1] = true;

        // Union the two nodes - keeps islands up to date automatically
        _unionFind.Union(idx1, idx2);
    }

    // Get all islands (connected components) - O(n * α(n))
    public IEnumerable<List<Node>> GetIslands()
    {
        var islandMap = new Dictionary<int, List<Node>>();

        for (var i = 0; i < _nodeCount; i++)
        {
            var root = _unionFind.Find(i);
            if (!islandMap.ContainsKey(root))
            {
                islandMap[root] = [];
            }

            islandMap[root].Add(_nodes[i]);
        }

        return islandMap.Values;
    }

    // Get the number of islands - O(n * α(n))
    public int GetIslandCount()
    {
        return _unionFind.GetComponentCount();
    }

    private static double CalculateDistance(Node n1, Node n2)
    {
        var dx = n2.X - n1.X;
        var dy = n2.Y - n1.Y;
        var dz = n2.Z - n1.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
}

// Union-Find data structure for efficient connected component tracking
public class UnionFind
{
    private readonly int[] _parent;
    private readonly int[] _rank;
    private int _componentCount;

    public UnionFind(int size)
    {
        _parent = new int[size];
        _rank = new int[size];
        _componentCount = size;

        for (var i = 0; i < size; i++)
        {
            _parent[i] = i;
            _rank[i] = 0;
        }
    }

    // Find with path compression - O(α(n)) amortized
    public int Find(int x)
    {
        if (_parent[x] != x)
        {
            _parent[x] = Find(_parent[x]); // Path compression
        }

        return _parent[x];
    }

    // Union by rank - O(α(n)) amortized
    public void Union(int x, int y)
    {
        var rootX = Find(x);
        var rootY = Find(y);

        if (rootX == rootY)
            return; // Already connected

        // Union by rank
        if (_rank[rootX] < _rank[rootY])
        {
            _parent[rootX] = rootY;
        }
        else if (_rank[rootX] > _rank[rootY])
        {
            _parent[rootY] = rootX;
        }
        else
        {
            _parent[rootY] = rootX;
            _rank[rootX]++;
        }

        _componentCount--;
    }

    public int GetComponentCount() => _componentCount;
}
