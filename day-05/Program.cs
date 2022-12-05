var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var drawing = input.TakeWhile((line) => line.Length > 0).Select((line) =>
    line.Chunk(4).Select(s => s.FirstOrDefault(char.IsAsciiLetterOrDigit)).ToList()
).ToList();

var steps = input.Skip(drawing.Count + 1).Select((line) =>
{
    var components = line.Split(" ");
    return (
        quantity: int.Parse(components[1]),
        source: int.Parse(components[3]),
        destination: int.Parse(components[5])
    );
});

{
    var stacks = CreateStacks();
    foreach (var step in steps)
    {
        for (var i = 0; i < step.quantity; i++)
        {
            stacks[step.destination - 1].Push(stacks[step.source - 1].Pop());
        }
    }

    Console.WriteLine($"Creates at top of each stack using CreateMover 9000: {new string(stacks.Select(s => s.Peek()).ToArray())}"); // VRWBSFZWM
}

{
    var stacks = CreateStacks();
    foreach (var step in steps)
    {
        var creates = new Stack<char>();
        for (var i = 0; i < step.quantity; i++)
        {
            creates.Push(stacks[step.source - 1].Pop());
        }

        foreach (var create in creates)
        {
            stacks[step.destination - 1].Push(create);
        }
    }

    Console.WriteLine($"Creates at top of each stack using CreateMover 9001: {new string(stacks.Select(s => s.Peek()).ToArray())}"); // RBTWJWMCF
}

List<Stack<char>> CreateStacks()
{
    var stacks = new List<Stack<char>>();
    for (var i = 0; i < drawing[0].Count; i++)
    {
        stacks.Add(new Stack<char>());
    }

    for (var i = drawing.Count - 1; i >= 0; i--)
    {
        for (var j = 0; j < drawing[i].Count; j++)
        {
            if (drawing[i][j] != default)
            {
                stacks[j].Push(drawing[i][j]);
            }
        }
    }

    return stacks;
}