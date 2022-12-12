var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadAllText(inputPath);

var monkeys = input.Split("\n\n").Select(Monkey.Parse);

Console.WriteLine($"Monkey business after 20 rounds: {MonkeyBusiness(rounds: 20, x => x / 3)}"); // 56120

var leastCommonMultiple = monkeys.Select(m => m.Divisor).Aggregate((a, x) => a * x);
Console.WriteLine($"Monkey business after 10,000 rounds: {MonkeyBusiness(rounds: 10_000, x => x % leastCommonMultiple)}"); // 24389045529

long MonkeyBusiness(int rounds, Func<long, long> reduceWorryLevelFunc)
{
    var monkeyStates = monkeys.Select(m => new MonkeyState(m)).ToList();

    for (var round = 0; round < rounds; round++)
    {
        foreach (var monkeyState in monkeyStates)
        {
            while (monkeyState.Items.TryDequeue(out var currentWorryLevel))
            {
                monkeyState.ItemsInspected++;

                var afterInspectionWorryLevel = monkeyState.Monkey.InspectItem(currentWorryLevel);

                var reducedWorryLevel = reduceWorryLevelFunc(afterInspectionWorryLevel);

                var catcher = monkeyState.Monkey.MonkeyToThrowTo(reducedWorryLevel);
                monkeyStates[catcher].Items.Enqueue(reducedWorryLevel);
            }
        }
    }

    return monkeyStates.Select(m => m.ItemsInspected).OrderDescending().Take(2).Aggregate((a, x) => a * x);
}

class MonkeyState
{
    public MonkeyState(Monkey monkey)
    {
        Monkey = monkey;
        Items = new Queue<long>(monkey.StartingItems);
    }

    public Monkey Monkey { get; init; }
    public Queue<long> Items { get; init; }
    public long ItemsInspected { get; set; }
}

class Monkey
{
    private readonly Func<long, long> operation;
    private readonly int throwToIfTrue;
    private readonly int throwToIfFalse;

    public Monkey(int number, IEnumerable<long> startingItems, Func<long, long> operation, long divisor, int throwToIfTrue, int throwToIfFalse)
    {
        Number = number;
        StartingItems = startingItems;
        this.operation = operation;
        Divisor = divisor;
        this.throwToIfTrue = throwToIfTrue;
        this.throwToIfFalse = throwToIfFalse;
    }

    public int Number { get; init; }

    public IEnumerable<long> StartingItems { get; init; }

    public long Divisor { get; init; }

    public long InspectItem(long worryLevel) => operation(worryLevel);

    public int MonkeyToThrowTo(long worryLevel)
    {
        return worryLevel % Divisor == 0 ? throwToIfTrue : throwToIfFalse;
    }

    public static Monkey Parse(string s)
    {
        var lines = s.Split("\n");

        var number = int.Parse(lines[0][7..^1]);
        var startingItems = lines[1][18..].Split(",").Select(long.Parse);
        var operation = lines[2][23..].Split(" ") switch
        {
            ["*", "old"] => (Func<long, long>)((old) => old * old),
            ["*", var x] => (Func<long, long>)((old) => old * long.Parse(x)),
            ["+", var x] => (Func<long, long>)((old) => old + long.Parse(x)),
            _ => throw new ArgumentException("Unhandled operation"),
        }; ;
        var divisor = long.Parse(lines[3][21..]);
        var throwToIfTrue = int.Parse(lines[4][29..]);
        var throwToIfFalse = int.Parse(lines[5][30..]);

        return new Monkey(number, startingItems, operation, divisor, throwToIfTrue, throwToIfFalse);
    }
}
