var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var cycle = 1;
var x = 1;

var signalStrengths = new List<int>();
var crt = new System.Text.StringBuilder();

foreach (var instruction in input)
{
    var (ticks, operand) = instruction switch
    {
        "noop" => (1, 0),
        _ => (2, int.Parse(instruction[5..])),
    };

    for (var t = 0; t < ticks; t++, cycle++)
    {
        if ((cycle % 40) == 20)
        {
            signalStrengths.Add(cycle * x);
        }

        var pixel = ((cycle - 1) % 40);
        var spriteIsVisible = pixel >= x - 1 && pixel <= x + 1;
        crt.Append(spriteIsVisible ? '#' : '.');
        if (pixel == 39)
        {
            crt.AppendLine();
        }
    }

    x += operand;
}

Console.WriteLine($"Sum of signal strengths: {signalStrengths.Sum()}"); // 17840
Console.WriteLine($"Eight capital letters that appear: \n{crt}"); // EALGULPG
