var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

{
    var rounds = input.Select(line => (
        opponent: line[0] switch
        {
            'A' => Shape.Rock,
            'B' => Shape.Paper,
            'C' => Shape.Scissors,
            _ => throw new ArgumentException("Invalid value for opponent")
        },
        self: line[2] switch
        {
            'X' => Shape.Rock,
            'Y' => Shape.Paper,
            'Z' => Shape.Scissors,
            _ => throw new ArgumentException("Invalid value for self")
        }
    ));

    var scores = rounds.Select(round =>
    {
        var outcome = round switch
        {
            { opponent: var o, self: var s } when o == s => Outcome.Draw,
            { opponent: Shape.Rock, self: Shape.Paper } => Outcome.Win,
            { opponent: Shape.Paper, self: Shape.Scissors } => Outcome.Win,
            { opponent: Shape.Scissors, self: Shape.Rock } => Outcome.Win,
            _ => Outcome.Lose
        }; ;

        return (int)round.self + (int)outcome;
    });

    Console.WriteLine($"Total score using second column as shape: {scores.Sum()}"); // 8392

}

{
    var rounds = input.Select(line => (
        opponent: line[0] switch
        {
            'A' => Shape.Rock,
            'B' => Shape.Paper,
            'C' => Shape.Scissors,
            _ => throw new ArgumentException("Invalid value for opponent")
        },
        outcome: line[2] switch
        {
            'X' => Outcome.Lose,
            'Y' => Outcome.Draw,
            'Z' => Outcome.Win,
            _ => throw new ArgumentException("Invalid value for outcome")
        }
    ));

    var scores = rounds.Select(round =>
    {
        var shape = round switch
        {
            { outcome: Outcome.Draw, opponent: var o } => o,
            { outcome: Outcome.Win, opponent: Shape.Rock } => Shape.Paper,
            { outcome: Outcome.Win, opponent: Shape.Paper } => Shape.Scissors,
            { outcome: Outcome.Win, opponent: Shape.Scissors } => Shape.Rock,
            { outcome: Outcome.Lose, opponent: Shape.Rock } => Shape.Scissors,
            { outcome: Outcome.Lose, opponent: Shape.Paper } => Shape.Rock,
            { outcome: Outcome.Lose, opponent: Shape.Scissors } => Shape.Paper,
            _ => throw new ArgumentException($"Unmatched round: {round}"),
        }; ;

        return (int)shape + (int)round.outcome;
    });

    Console.WriteLine($"Total score using second column as outcome: {scores.Sum()}"); // 10116
}

Alternate(input);

static void Alternate(IEnumerable<string> input)
{
    Console.WriteLine("\nAlternate implementation using modulus");

    {
        var rounds = input.Select(line => (
            opponent: line[0] - 'A',
            self: line[2] - 'X'
        ));

        var scores = rounds.Select(round =>
        {
            var outcome = round switch
            {
                _ when round.opponent == round.self => 3, // Draw
                _ when ((round.opponent + 1) % 3) == round.self => 6, // Win
                _ => 0, // Lose
            };

            return round.self + 1 + outcome;
        });

        Console.WriteLine($"Total score using second column as shape: {scores.Sum()}"); // 8392
    }

    {
        var rounds = input.Select(line => (
            opponent: line[0] - 'A',
            outcome: line[2] - 'X'
        ));

        var scores = rounds.Select(round =>
        {
            var self = round.outcome switch
            {
                0 => (round.opponent + 2) % 3, // Lose
                1 => round.opponent, // Draw
                2 => (round.opponent + 1) % 3, // Win
                _ => throw new ArgumentException($"Unmatched round outcome: {round.outcome}"),
            };

            return self + 1 + (round.outcome * 3);
        });

        Console.WriteLine($"Total score using second column as outcome: {scores.Sum()}"); // 10116
    }
}

enum Shape
{
    Rock = 1,
    Paper = 2,
    Scissors = 3,
}

enum Outcome
{
    Lose = 0,
    Draw = 3,
    Win = 6,
}
