var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

Console.WriteLine($"Sum of the grove coordinates: {GroveCoordinatesSum(mixCount: 1, decryptionKey: 1)}"); // 9687
Console.WriteLine($"Sum of the grove coordinates from decrypted numbers: {GroveCoordinatesSum(mixCount: 10, decryptionKey: 811589153)}"); // 1338310513297

long GroveCoordinatesSum(int mixCount, long decryptionKey)
{
    var numbers = input.Select((line, index) => new Number(long.Parse(line) * decryptionKey, index)).ToList();

    for (var mix = 0; mix < mixCount; mix++)
    {
        for (var i = 0; i < numbers.Count; i++)
        {
            var currentIndex = numbers.FindIndex(n => n.OriginalIndex == i);
            var number = numbers[currentIndex];

            var newIndex = currentIndex + number.Value;
            if (newIndex != 0)
            {
                newIndex %= (numbers.Count - 1);
                if (newIndex <= 0)
                {
                    newIndex += numbers.Count - 1;
                }
            }

            numbers.RemoveAt(currentIndex);
            numbers.Insert((int)newIndex, number);
        }
    }

    var offsets = new[] {
        1_000,
        2_000,
        3_000
    };

    var zeroIndex = numbers.FindIndex(n => n.Value == 0);
    return offsets.Select((offset) => numbers[(zeroIndex + offset) % numbers.Count].Value).Sum();
}

record struct Number(long Value, int OriginalIndex);
