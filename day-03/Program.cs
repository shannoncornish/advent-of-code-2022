var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

Console.WriteLine($"Sum of priorities for compartments of each rucksack: {input.Select(SplitToHalves).Select(Common).Select(Priority).Sum()}");
Console.WriteLine($"Sum of priorities for group of elves: {input.Chunk(3).Select(Common).Select(Priority).Sum()}");

char Common(string[] groups) => groups[0].First(item => groups[1..].All(group => group.Contains(item)));

int Priority(char item) => item switch
{
    >= 'A' and <= 'Z' => item - 'A' + 27,
    >= 'a' and <= 'z' => item - 'a' + 1,
    _ => 0,
};

string[] SplitToHalves(string s) => new[] { s[0..(s.Length / 2)], s[(s.Length / 2)..] };
