var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath).ToList();

var startingPositions = new HashSet<Position>();
for (var row = 0; row < input.Count; row++)
{
    for (var column = 0; column < input[row].Length; column++)
    {
        if (input[row][column] == '#')
        {
            startingPositions.Add(new Position(row + 1, column + 1));
        }
    }
}

var (finalPositions, _) = SimulateMoves(startingPositions, rounds: 10);

var rows = (finalPositions.Max(k => k.Row) - finalPositions.Min(k => k.Row)) + 1;
var columns = (finalPositions.Max(k => k.Column) - finalPositions.Min(k => k.Column)) + 1;

Console.WriteLine($"Ground tiles the smallest rectangle contains: {rows * columns - finalPositions.Count}"); // 4181

var (_, round) = SimulateMoves(startingPositions, rounds: int.MaxValue);

Console.WriteLine($"Number of the first round where no Elf moves: {round}"); // 973

(HashSet<Position> currentPositions, int round) SimulateMoves(HashSet<Position> elves, int rounds)
{
    const CardinalDirection NotFound = (CardinalDirection)(-1);

    var currentPositions = new HashSet<Position>(elves);

    for (var round = 0; round < rounds; round++)
    {
        var directions = Enumerable.Range(round, 4).Select(x => x % 4).Cast<CardinalDirection>();

        var proposedPositions = new Dictionary<Position, List<Position>>();
        foreach (var elf in currentPositions)
        {
            var adjacents = new Dictionary<CardinalDirection, Position[]>
            {
                { CardinalDirection.North, new Position[]
                    {
                        new(elf.Row - 1, elf.Column),     // N
                        new(elf.Row - 1, elf.Column + 1), // NE
                        new(elf.Row - 1, elf.Column - 1), // NW
                    }
                },
                {
                    CardinalDirection.South, new Position[]
                    {
                        new(elf.Row + 1, elf.Column),     // S
                        new(elf.Row + 1, elf.Column + 1), // SE
                        new(elf.Row + 1, elf.Column - 1), // SW
                    }
                },
                {
                    CardinalDirection.West, new Position[]
                    {
                        new(elf.Row, elf.Column - 1),     // W
                        new(elf.Row - 1, elf.Column - 1), // NW
                        new(elf.Row + 1, elf.Column - 1), // SW
                    }
                },
                {
                    CardinalDirection.East, new Position[]
                    {
                        new(elf.Row, elf.Column + 1),     // E
                        new(elf.Row - 1, elf.Column + 1), // NE
                        new(elf.Row + 1, elf.Column + 1), // SE
                    }
                }
            };

            if (adjacents.Values.SelectMany(x => x).Any(currentPositions.Contains))
            {
                var direction = directions.FirstOrDefault(d => !adjacents[d].Any(currentPositions.Contains), defaultValue: NotFound);
                if (direction == NotFound)
                {
                    continue;
                }

                var proposedPosition = adjacents[direction][0];

                var elvesProposingPosition = proposedPositions.GetValueOrDefault(proposedPosition);
                if (elvesProposingPosition is null)
                {
                    elvesProposingPosition = new List<Position>();
                    proposedPositions.Add(proposedPosition, elvesProposingPosition);
                }

                elvesProposingPosition.Add(elf);
            }
        }

        if (proposedPositions.Count == 0)
        {
            return (currentPositions, round + 1);
        }

        var movingPositions = proposedPositions
            .Where(kv => kv.Value.Count == 1)
            .Select(kv => (newPosition: kv.Key, oldPosition: kv.Value[0]));

        var newPositions = new HashSet<Position>(movingPositions.Select(x => x.newPosition));
        var oldPositions = new HashSet<Position>(movingPositions.Select(x => x.oldPosition));

        currentPositions = new HashSet<Position>(newPositions.Concat(currentPositions.Except(oldPositions)));
    }

    return (currentPositions, rounds);
}

record struct Position(int Row, int Column);

enum CardinalDirection
{
    North,
    South,
    West,
    East,
}
