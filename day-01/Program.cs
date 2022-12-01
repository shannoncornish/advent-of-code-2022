var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var calories = new List<int> { 0 };
var i = 0;

foreach (var line in input)
{
    if (line == "")
    {
        i++;
        calories.Add(0);
    }
    else
    {
        calories[i] += int.Parse(line);
    }
}

var orderedCalories = calories.OrderDescending();

Console.WriteLine($"Total calories the of the top elf: {orderedCalories.First()}"); // 75501
Console.WriteLine($"Total calories of the top three elves: {orderedCalories.Take(3).Sum()}"); // 215594
