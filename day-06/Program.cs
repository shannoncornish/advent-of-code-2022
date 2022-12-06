var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadAllText(inputPath);

Console.WriteLine($"Characters before the first start-of-packet marker: {IndexOfDistinctWindow(input, 4)}"); // 1658
Console.WriteLine($"Characters before the first start-of-message marker: {IndexOfDistinctWindow(input, 14)}"); // 2260

int IndexOfDistinctWindow(string s, int size) =>
    Enumerable.Range(size, s.Length).FirstOrDefault(i => s.Substring(i - size, size).Distinct().Count() == size, -1);

/*
 * Alternate implementations using a for loop.

int IndexOfDistinctWindow(string s, int size)
{
    for (var i = size; i < s.Length; i++)
    {
        if (s.Substring(i - size, size).Distinct().Count() == size)
        {
            return i;
        }
    }

    return -1;
}

int IndexOfDistinctWindow(string s, int size)
{
    for (var i = 0; i < s.Length - size; i++)
    {
        if (s.Substring(i, size).Distinct().Count() == size)
        {
            return i + size;
        }
    }

    return -1;
}
*/
