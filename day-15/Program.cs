var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

// NOTE: This uses constant values that are only valid when running with the real input.

var integerRegex = new System.Text.RegularExpressions.Regex("-?\\d+");

var sensors = input.Select((line) =>
{
    var matches = integerRegex.Matches(line).Select(m => int.Parse(m.Value)).ToArray();
    return new Sensor(new Point(matches[0], matches[1]), new Beacon(new Point(matches[2], matches[3])));
});

var sensorAreas = sensors.Select(s => new SensorArea(s));

{
    const int Row = 2_000_000;

    var grid = new Dictionary<Point, char>();
    foreach (var sensor in sensors)
    {
        if (sensor.Location.Y == Row)
        {
            grid[sensor.Location] = 'S';
        }

        if (sensor.NearestBeacon.Location.Y == Row)
        {
            grid[sensor.NearestBeacon.Location] = 'B';
        }
    }

    foreach (var sensorArea in sensorAreas)
    {
        var perimeter = sensorArea.GetPerimeter();
        var pointsFromPerimeterInRow = perimeter.Where(p => p.Y == Row).ToList();
        if (pointsFromPerimeterInRow.Any())
        {
            var pointsInRow = pointsFromPerimeterInRow[0].GetPointsTo(pointsFromPerimeterInRow[1]);
            foreach (var point in pointsInRow)
            {
                grid.TryAdd(point, '#');
            }
        }
    }

    Console.WriteLine($"Positions that cannot contain a beacon: {grid.Where(p => p.Value == '#').Count()}"); // 4560025
}

{
    var searchArea = new Rect(new Point(0, 0), new Point(4_000_000, 4_000_000));

    var outsidePerimeterPoints = new Dictionary<Point, int>();
    foreach (var sensorArea in sensorAreas)
    {
        foreach (var outsidePoint in sensorArea.GetOutsidePerimeter().Where(searchArea.Contains))
        {
            outsidePerimeterPoints.TryGetValue(outsidePoint, out var count);
            outsidePerimeterPoints[outsidePoint] = count + 1;
        }
    }

    var distressBeacon = new Point(-1, -1);
    foreach (var (outsidePoint, _) in outsidePerimeterPoints.Where(pair => pair.Value > 2))
    {
        var outsidePointDetectedBySensor = false;
        foreach (var sensor in sensors)
        {
            var distanceToOutsidePoint = sensor.Location.DistanceTo(outsidePoint);
            if (distanceToOutsidePoint < sensor.DistanceToNearestBeacon)
            {
                outsidePointDetectedBySensor = true;
                break;
            }
        }

        if (!outsidePointDetectedBySensor)
        {
            distressBeacon = outsidePoint;
            break;
        }
    }

    var tuningFrequency = distressBeacon.X * 4_000_000L + distressBeacon.Y;
    Console.WriteLine($"Tuning frequency of the distress beacon: {tuningFrequency}"); // 12480406634249
}

record struct SensorArea(Sensor Sensor)
{
    public Point Top { get; } = Sensor.Location with { Y = Sensor.Location.Y - Sensor.DistanceToNearestBeacon };
    public Point Bottom { get; } = Sensor.Location with { Y = Sensor.Location.Y + Sensor.DistanceToNearestBeacon };
    public Point Left { get; } = Sensor.Location with { X = Sensor.Location.X - Sensor.DistanceToNearestBeacon };
    public Point Right { get; } = Sensor.Location with { X = Sensor.Location.X + Sensor.DistanceToNearestBeacon };

    public IEnumerable<Point> GetPerimeter()
    {
        return Top.GetPointsTo(Right)
            .Concat(Right.GetPointsTo(Bottom)
            .Concat(Bottom.GetPointsTo(Left)
            .Concat(Left.GetPointsTo(Top))));
    }

    public IEnumerable<Point> GetOutsidePerimeter()
    {
        var outsideTop = Top with { Y = Top.Y - 1 };
        var outsideBottom = Bottom with { Y = Top.Y + 1 };
        var outsideLeft = Left with { X = Left.X - 1 };
        var outsideRight = Right with { X = Right.X + 1 };

        return outsideTop.GetPointsTo(outsideRight)
            .Concat(outsideRight.GetPointsTo(outsideBottom)
            .Concat(outsideBottom.GetPointsTo(outsideLeft)
            .Concat(outsideLeft.GetPointsTo(outsideTop))));
    }
}

record struct Sensor(Point Location, Beacon NearestBeacon)
{
    public int DistanceToNearestBeacon { get; } = Location.DistanceTo(NearestBeacon.Location);
}

record struct Beacon(Point Location);

record struct Point(int X, int Y)
{
    public int DistanceTo(Point p)
    {
        return Math.Abs(p.X - X) + Math.Abs(p.Y - Y);
    }

    public IEnumerable<Point> GetPointsTo(Point p)
    {
        var diffX = p.X - X;
        var diffY = p.Y - Y;

        var dx = Math.Sign(diffX);
        var dy = Math.Sign(diffY);

        var count = Math.Max(Math.Abs(diffX), Math.Abs(diffY)) + 1;
        for (var i = 0; i < count; i++)
        {
            yield return new Point(X + (dx * i), Y + (dy * i));
        }
    }
}

record struct Rect(Point Min, Point Max)
{
    public bool Contains(Point p)
    {
        return p.X >= Min.X && p.X <= Max.X && p.Y >= Min.Y && p.Y <= Max.Y;
    }
}
