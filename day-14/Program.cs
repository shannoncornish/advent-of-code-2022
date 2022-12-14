var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var rockPaths = input.Select(line =>
{
    var points = line.Split(" -> ");
    return points.Zip(points.Skip(1)).SelectMany(x =>
        Point.Parse(x.First).GetPoints(Point.Parse(x.Second)));
});

var rocks = rockPaths.SelectMany(p => p).Distinct();

var minY = 0; 
var maxY = rocks.Max(r => r.Y);

var sandSpawnPoint = new Point(500, 0);

{
    var area = new Area(
        Min: new Point(X: rocks.Min(r => r.X), Y: minY),
        Max: new Point(X: rocks.Max(r => r.X), Y: maxY)
        );

    Console.WriteLine($"Units of sand come to rest before abyss: {SimulateWhileSandInArea(rocks, area)}"); // P1: 825
}

{
    var floorY = maxY + 2;
    var floor = Enumerable.Range(0, 1000).Select(x => new Point(x, floorY));

    var area = new Area(
        Min: new Point(X: 0, Y: minY),
        Max: new Point(X: 1000, Y: floorY)
        );

    Console.WriteLine($"Units of sand come to rest: {SimulateWhileSandInArea(rocks.Concat(floor), area)}"); // P2: 26729
}

int SimulateWhileSandInArea(IEnumerable<Point> rocks, Area area)
{
    var stationary = rocks.ToDictionary(p => p, _ => '#');

    var movements = new[] { 0, -1, 1 };

    var stationarySandCount = 0;
    while (true)
    {
        var sand = sandSpawnPoint;

        var sandMoving = true;
        while (sandMoving)
        {
            var next = movements.Select(dx => new Point(sand.X + dx, sand.Y + 1)).FirstOrDefault(p => !stationary.ContainsKey(p), new Point(-1,-1));
            if (next != new Point(-1, -1))
            {
                if (area.Contains(next))
                {
                    sand = next;
                    continue;
                }

                return stationarySandCount;
            }

            sandMoving = false;
        }

        stationary[sand] = 'o';
        stationarySandCount++;

        if (sand == sandSpawnPoint)
        {
            return stationarySandCount;
        }
    }
}

record struct Point(int X, int Y)
{
    public static Point Parse(string s)
    {
        var coordinates = s.Split(",");
        return new Point(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
    }

    public IEnumerable<Point> GetPoints(Point p)
    {
        if (X == p.X)
        {
            var min = Math.Min(Y, p.Y);
            var max = Math.Max(Y, p.Y);

            return Enumerable.Range(min, max - min + 1).Select(y => new Point(p.X, y));
        }
        else
        {
            var min = Math.Min(X, p.X);
            var max = Math.Max(X, p.X);

            return Enumerable.Range(min, max - min + 1).Select(x => new Point(x, p.Y));
        }
    }
}

record struct Area(Point Min, Point Max)
{
    public bool Contains(Point p)
    {
        return p.X >= Min.X && p.X <= Max.X && p.Y >= Min.Y && p.Y <= Max.Y;
    }
}
