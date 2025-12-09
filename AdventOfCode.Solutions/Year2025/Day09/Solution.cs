using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Solutions.Year2025.Day09;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
public class Solution : ISolution
{
    public string SolvePartOne(string input)
    {
        var coordinates = GetCoordinates(input);
        return GetRectangles(coordinates)
            .Aggregate(0L, (largest, rectangle) => Math.Max(largest, rectangle.Area))
            .ToString();
    }

    public string SolvePartTwo(string input)
    {
        var coordinates = GetCoordinates(input).ToArray();
        var edges = GetEdges(coordinates).ToArray();

        PriorityQueue<Rectangle, long>
            heap = new(
                GetRectangles(coordinates).Select(rectangle => (rectangle, rectangle.Area)),
                Comparer<long>.Create((x, y) => y.CompareTo(x))
            );

        while (heap.TryDequeue(out var rectangle, out var area))
            if (edges.All(edge => !rectangle.Intersect(edge)))
                return area.ToString();

        throw new Exception("No solution found");
    }

    private static IEnumerable<Point> GetCoordinates(string input)
        => input
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var coordinates = line.Split(",").Select(long.Parse).ToArray();
                return new Point(coordinates[0], coordinates[1]);
            });

    private static IEnumerable<Line> GetEdges(IEnumerable<Point> coordinates)
    {
        var arr = coordinates.ToArray();
        yield return new Line(arr[^1], arr[0]);
        for (var i = 0; i < arr.Length - 1; i++)
            yield return new Line(arr[i], arr[i + 1]);
    }


    private static IEnumerable<Rectangle> GetRectangles(IEnumerable<Point> coordinates)
    {
        var arr = coordinates.ToArray();
        for (var i = 0; i < arr.Length - 1; i++)
        for (var j = i + 1; j < arr.Length; j++)
            yield return new Rectangle(arr[i], arr[j]);
    }
}

public record Point(long X, long Y);

public class Line(Point start, Point end)
{
    private Point Start { get; } = start;
    private Point End { get; } = end;

    private static int Orientation(Point p, Point q, Point r)
    {
        var val = (q.Y - p.Y) * (r.X - q.X) -
                  (q.X - p.X) * (r.Y - q.Y);

        if (val == 0) return 0;
        return val > 0 ? 1 : 2;
    }

    private static bool OnSegment(Point p, Point q, Point r)
    {
        return q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
               q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y);
    }

    public bool Intersect(Line other)
    {
        var o1 = Orientation(Start, End, other.Start);
        var o2 = Orientation(Start, End, other.End);
        var o3 = Orientation(other.Start, other.End, Start);
        var o4 = Orientation(other.Start, other.End, End);

        if (o1 != o2 && o3 != o4) return true;

        var s1 = o1 == 0 && OnSegment(Start, other.Start, End);
        var s2 = o2 == 0 && OnSegment(Start, other.End, End);
        var s3 = o3 == 0 && OnSegment(other.Start, Start, other.End);
        var s4 = o4 == 0 && OnSegment(other.Start, End, other.End);

        return s1 || s2 || s3 || s4;
    }
}

public class Rectangle
{
    private long Top { get; }
    private long Bottom { get; }
    private long Left { get; }
    private long Right { get; }

    private Line[] Edges { get; }

    public long Area { get; }

    public Rectangle(Point a, Point b)
    {
        Top = Math.Min(a.Y, b.Y);
        Bottom = Math.Max(a.Y, b.Y);
        Left = Math.Min(a.X, b.X);
        Right = Math.Max(a.X, b.X);

        // INNER rectangle edges (1 unit inside)
        var tl = new Point(Left + 1, Top + 1);
        var tr = new Point(Right - 1, Top + 1);
        var bl = new Point(Left + 1, Bottom - 1);
        var br = new Point(Right - 1, Bottom - 1);
        Edges =
        [
            new Line(tl, tr), // top
            new Line(tr, br), // right
            new Line(br, bl), // bottom
            new Line(bl, tl) // left
        ];

        var width = Math.Max(0, Right - Left + 1);
        var height = Math.Max(0, Bottom - Top + 1);

        Area = width * height;
    }

    public bool Intersect(Line line)
        => Edges.Any(edge => edge.Intersect(line));
}