var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var elevations = input.Select(line => line.ToList()).ToList();

var none = new Point(-1, -1);
var start = none;
var end = none;

var rows = elevations.Count;
var columns = elevations[0].Count;

for (var r = 0; r < rows; r++)
{
    var c = elevations[r].IndexOf('S');
    if (c != -1)
    {
        start = new Point(r, c);
        elevations[r][c] = 'a';
    }

    c = elevations[r].IndexOf('E');
    if (c != -1)
    {
        end = new Point(r, c);
        elevations[r][c] = 'z';
    }

    if (start != none && end != none)
    {
        break;
    }
}

var frontier = new Queue<Point>();
frontier.Enqueue(end);

var distanceFromEnd = new Dictionary<Point, int>();
distanceFromEnd.Add(end, 0);

while (frontier.TryDequeue(out var current))
{
    var neighbors = new List<Point>();
    foreach (var neighbor in new[] {
        current with { Row = current.Row - 1 },
        current with { Row = current.Row + 1 },
        current with { Column = current.Column - 1 },
        current with { Column = current.Column + 1 }})
    {
        if (neighbor.Row >= 0 && neighbor.Row < rows
            && neighbor.Column >= 0 && neighbor.Column < columns
            && elevations[current.Row][current.Column] - elevations[neighbor.Row][neighbor.Column] <= 1)
        {
            if (!distanceFromEnd.ContainsKey(neighbor))
            {
                frontier.Enqueue(neighbor);
                distanceFromEnd.Add(neighbor, distanceFromEnd[current] + 1);
            }
        }
    }
}

Console.Write("Fewest steps required to move from start to end: ");
Console.WriteLine(distanceFromEnd[start]); // 352

Console.Write("Fewest steps required to move from any lowest elevation to end: ");
Console.WriteLine(distanceFromEnd.Where((pair) => elevations[pair.Key.Row][pair.Key.Column] == 'a').Min((pair) => pair.Value)); // 345

record struct Point(int Row, int Column);
