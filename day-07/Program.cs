var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var fileSizes = new Dictionary<string, int>
{
    { "/", 0 },
};

var path = "/";
foreach (var line in input)
{
    switch (line[0])
    {
        case '$':
            if (line[2] == 'c')
            {
                var argument = line[5..];
                switch (argument)
                {
                    case "/":
                        path = "/";
                        break;
                    case "..":
                        var i = path.LastIndexOf('/');
                        path = i == 0 ? "/" : path[..i];
                        break;
                    default:
                        path = Path.Join(path, argument);
                        break;
                }
            }
            break;
        case 'd':
            var name = line[4..];
            fileSizes.Add(Path.Join(path, name), 0);
            break;
        case >= '0' and <= '9':
            var size = int.Parse(line[..line.IndexOf(' ')]);
            fileSizes[path] += size;
            break;
    }
}

var directorySizes = new Dictionary<string, int>();
foreach (var directory in fileSizes.Keys)
{
    var prefix = directory == "/" ? directory : $"{directory}/";
    var size = fileSizes
        .Where((pair) => pair.Key == directory || pair.Key.StartsWith(prefix))
        .Sum((pair) => pair.Value);

    directorySizes[directory] = size;
}

Console.WriteLine($"Sum of directories under 100,000: {directorySizes.Values.Where(size => size <= 100_000).Sum()}"); // 1350966

const int TotalDiskSize = 70_000_000;
const int UpdateSize = 30_000_000;

var requiredSize = UpdateSize - (TotalDiskSize - directorySizes["/"]);
Console.WriteLine($"Size of smallest directory to delete: {directorySizes.Values.Where(size => size >= requiredSize).Min()}"); // 6296435
