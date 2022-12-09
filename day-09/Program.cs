var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

const int NumberOfKnots = 10;

var knotPositions = new List<Stack<(int x, int y)>>();
for (var i = 0; i < NumberOfKnots; i++)
{
    var stack = new Stack<(int x, int y)>();
    stack.Push((x: 0, y: 0));
    knotPositions.Add(stack);
}

foreach (var line in input)
{
    var direction = line[0];
    var steps = int.Parse(line[2..]);

    for (var step = 0; step < steps; step++)
    {
        // Move head
        var head = knotPositions[0].Peek();
        knotPositions[0].Push(direction switch
        {
            'U' => (x: head.x, y: head.y + 1),
            'D' => (x: head.x, y: head.y - 1),
            'L' => (x: head.x - 1, y: head.y),
            'R' => (x: head.x + 1, y: head.y),
            _ => throw new ArgumentException($"Invalid direction: {direction}")
        });

        // Move tail
        for (var i = 1; i < NumberOfKnots; i++)
        {
            var previous = knotPositions[i - 1].Peek();
            var current = knotPositions[i].Peek();

            var distance = (x: previous.x - current.x, y: previous.y - current.y);

            var moveTo = current;
            if (Math.Abs(distance.x) > 1)
            {
                moveTo.x += Math.Sign(distance.x);
                if (distance.y != 0)
                {
                    moveTo.y += Math.Sign(distance.y);
                }
            }
            else if (Math.Abs(distance.y) > 1)
            {
                moveTo.y += Math.Sign(distance.y);
                if (distance.x != 0)
                {
                    moveTo.x += Math.Sign(distance.x);
                }
            }

            if (moveTo != current)
            {
                knotPositions[i].Push(moveTo);
            }
            else
            {
                // Current knot did not move so no additional moves in the tail
                break;
            }
        }
    }
}

Console.WriteLine($"Unique positions visited by the tail of a 2 knot rope: {knotPositions[1].ToHashSet().Count}"); // 6486
Console.WriteLine($"Unique positions visited by the tail of a 10 knot rope: {knotPositions[9].ToHashSet().Count}"); // 2678
