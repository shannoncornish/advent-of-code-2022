var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var points = input.Select(Point.Parse).ToHashSet();

var surfaceArea = 0;
foreach (var p in points)
{
    var surfaces = new[]
    {
        p with { X = p.X + 1 },
        p with { X = p.X - 1 },
        p with { Y = p.Y + 1 },
        p with { Y = p.Y - 1 },
        p with { Z = p.Z + 1 },
        p with { Z = p.Z - 1 },
    };

    surfaceArea += 6 - surfaces.Count(points.Contains);
};

Console.WriteLine($"Surface area of scanned lava droplet: {surfaceArea}"); // P1: 4548

var orderedX = points.Select(p => p.X).Order();
var orderedY = points.Select(p => p.Y).Order();
var orderedZ = points.Select(p => p.Z).Order();

var min = new Point(orderedX.First() - 1, orderedY.First() - 1, orderedZ.First() - 1);
var max = new Point(orderedX.Last() + 1, orderedY.Last() + 1, orderedZ.Last() + 1);

var area = new Area(min, max);

var frontier = new Queue<Point>();
frontier.Enqueue(min);

var reached = new HashSet<Point>();
reached.Add(min);

var exteriorSurfaceArea = 0;
while (frontier.TryDequeue(out var current))
{
    var neighbors = new[]
    {
        current with { X = current.X + 1 },
        current with { X = current.X - 1 },
        current with { Y = current.Y + 1 },
        current with { Y = current.Y - 1 },
        current with { Z = current.Z + 1 },
        current with { Z = current.Z - 1 },
    };

    foreach (var neighbor in neighbors)
    {
        if (area.Contains(neighbor) && !reached.Contains(neighbor))
        {
            if (points.Contains(neighbor))
            {
                exteriorSurfaceArea++;
                continue;
            }

            frontier.Enqueue(neighbor);
            reached.Add(neighbor);
        }
    }
}

Console.WriteLine($"Exterior surface area of scanned lava droplet: {exteriorSurfaceArea}"); // P2: 2588

record struct Point(int X, int Y, int Z)
{
    public static Point Parse(string s)
    {
        var xyz = s.Split(",");
        return new Point(int.Parse(xyz[0]), int.Parse(xyz[1]), int.Parse(xyz[2]));
    }
}

record struct Area(Point Min, Point Max)
{
    public bool Contains(Point p)
    {
        return p.X >= Min.X && p.X <= Max.X && p.Y >= Min.Y && p.Y <= Max.Y && p.Z >= Min.Z && p.Z <= Max.Z;
    }
}
