var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath).ToList();

var walls = new HashSet<Position>();
var blizzards = new HashSet<Blizzard>();

var start = new Position(-1, -1);
var end = new Position(-1, -1);

var insideRowCount = input.Count - 2;
var insideColumnCount = input[0].Length - 2;

var blizzardPositionsCache = new Dictionary<int, List<Position>>();

for (var row = 0; row < input.Count; row++)
{
    for (var column = 0; column < input[0].Length; column++)
    {
        var position = new Position(row + 1, column + 1);
        switch (input[row][column])
        {
            case '#':
                walls.Add(position);
                break;
            case '.':
                if (row == 0)
                {
                    start = position;
                    walls.Add(position with { Row = position.Row - 1 });
                }
                if (row == input.Count - 1)
                {
                    end = position;
                    walls.Add(position with { Row = position.Row + 1 });
                }
                break;
            case '^':
                blizzards.Add(new Blizzard(Direction.Up, position));
                break;
            case 'v':
                blizzards.Add(new Blizzard(Direction.Down, position));
                break;
            case '<':
                blizzards.Add(new Blizzard(Direction.Left, position));
                break;
            case '>':
                blizzards.Add(new Blizzard(Direction.Right, position));
                break;
            default:
                throw new ArgumentException($"Unhandled tile '{input[row][column]}' at position: {position}");
        }
    }
}

var trip1 = NavigateToEnd(start, end, 0);
var trip2 = NavigateToEnd(end, start, trip1);
var trip3 = NavigateToEnd(start, end, trip2);

Console.WriteLine($"Fewest number of minutes required: {trip1}"); // 305
Console.WriteLine($"Fewest number of minutes required with going back for snacks: {trip3}"); // 905

int NavigateToEnd(Position start, Position end, int startMinute)
{
    var frontier = new PriorityQueue<Expedition, int>();
    frontier.Enqueue(new Expedition(start, startMinute), startMinute);

    var checkpoints = new HashSet<(Position position, int minute)>();

    while (frontier.TryDequeue(out var current, out var _))
    {
        if (current.Position == end)
        {
            return current.Minute;
        }

        var checkpoint = (current.Position, current.Minute % (insideRowCount * insideColumnCount));
        if (!checkpoints.Add(checkpoint))
        {
            continue;
        }

        var moves = new Position[]
        {
            current.Position with { Row = current.Position.Row - 1 }, // Up
            current.Position with { Row = current.Position.Row + 1 }, // Down
            current.Position with { Column = current.Position.Column - 1 }, // Left
            current.Position with { Column = current.Position.Column + 1 }, // Right
            current.Position,
        };

        var futureBlizzardPositions = new HashSet<Position>(GetBlizzardPositions(current.Minute + 1));

        foreach (var move in moves)
        {
            if (!futureBlizzardPositions.Contains(move) && !walls.Contains(move))
            {
                var priority = (Distance(move, end) * 2) + current.Minute + 1;
                frontier.Enqueue(new Expedition(move, current.Minute + 1), priority);
            }
        }
    }

    return -1;
}

int Distance(Position x, Position y)
{
    return int.Abs(x.Row - y.Row) + int.Abs(x.Column - y.Column);
}

IEnumerable<Position> GetBlizzardPositions(int minute)
{
    var offset = minute % (insideRowCount * insideColumnCount);
    if (blizzardPositionsCache.TryGetValue(offset, out var positions))
    {
        return positions;
    }

    const int ModOffset = 2; // Positions are 1 based, +1 for the the valley wall

    positions = new List<Position>();
    foreach (var blizzard in blizzards)
    {
        var row = blizzard.InitialPosition.Row - ModOffset;
        var column = blizzard.InitialPosition.Column - ModOffset;
        switch (blizzard.Direction)
        {
            case Direction.Up:
                positions.Add(blizzard.InitialPosition with
                {
                    Row = ((((row - minute) % insideRowCount) + insideRowCount) % insideRowCount) + ModOffset
                });
                break;
            case Direction.Down:
                positions.Add(blizzard.InitialPosition with
                {
                    Row = ((row + minute) % insideRowCount) + ModOffset
                });
                break;
            case Direction.Left:
                positions.Add(blizzard.InitialPosition with
                {
                    Column = ((((column - minute) % insideColumnCount) + insideColumnCount) % insideColumnCount) + ModOffset
                });
                break;
            case Direction.Right:
                positions.Add(blizzard.InitialPosition with
                {
                    Column = ((column + minute) % insideColumnCount) + ModOffset
                });
                break;
        }
    }

    blizzardPositionsCache[offset] = positions;
    return positions;
}

record struct Expedition(Position Position, int Minute);

record struct Blizzard(Direction Direction, Position InitialPosition);

record struct Position(int Row, int Column);

enum Direction
{
    Up,
    Down,
    Left,
    Right,
}
