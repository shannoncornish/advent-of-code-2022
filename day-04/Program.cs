var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var assignments = input.Select((line) =>
{
    var sections = line.Split(",").SelectMany((pair) => pair.Split("-")).Select(int.Parse).ToArray();
    return (first: new Range(sections[0], sections[1]), second: new Range(sections[2], sections[3]));
});

Console.WriteLine($"assignment pairs where range contains other: {assignments.Count(Contains)}"); // 595

Console.WriteLine($"assignment pairs where range overlaps: {assignments.Count(Overlaps)}"); // 952

bool Contains((Range first, Range second) pair)
{
    return ((pair.first.Start.Value >= pair.second.Start.Value && pair.first.End.Value <= pair.second.End.Value)
        || (pair.second.Start.Value >= pair.first.Start.Value && pair.second.End.Value <= pair.first.End.Value));
}

bool Overlaps((Range first, Range second) pair)
{
    return ((pair.first.Start.Value >= pair.second.Start.Value && pair.first.Start.Value <= pair.second.End.Value)
        || (pair.first.End.Value >= pair.second.Start.Value && pair.first.End.Value <= pair.second.End.Value)
        || (pair.second.Start.Value >= pair.first.Start.Value && pair.second.Start.Value <= pair.first.End.Value)
        || (pair.second.End.Value >= pair.first.Start.Value && pair.second.End.Value <= pair.first.End.Value));
}
