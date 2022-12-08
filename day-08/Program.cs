var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadLines(inputPath);

var grid = input.Select(line => line).ToList();

var visible = new HashSet<(int row, int column)>();
var scores = new Dictionary<(int row, int column), (int up, int down, int left, int right)>();

for (var row = 0; row < grid.Count; row++)
{
    for (var column = 0; column < grid.Count; column++)
    {
        if (IsVisibleFromOutside(row, column))
        {
            visible.Add((row, column));
        }

        scores.Add((row, column), ScenicScore(row, column));
    }
}

Console.WriteLine($"Trees visible from outside the grid: {visible.Count}"); // 1809
Console.WriteLine($"Highest scenic score: {scores.Values.Max(v => v.up * v.down * v.left * v.right)}"); // 479400

bool IsVisibleFromOutside(int row, int column)
{
    var value = grid[row][column];

    if (grid[row].Take(column).All(x => x < value) // Left
        || (grid[row].Skip(column + 1).All(x => x < value))) // Right
    {
        return true;
    }

    var size = grid.Count - 1;
    if (Enumerable.Range(0, row).All(row => grid[row][column] < value) // Up
        || (Enumerable.Range(row + 1, size - row).All(row => grid[row][column] < value))) // Down
    {
        return true;
    }

    return false;
}

(int up, int down, int left, int right) ScenicScore(int row, int column)
{
    var size = grid.Count - 1;

    var value = grid[row][column];

    var left = 0;
    for (var i = column - 1; i >= 0; i--)
    {
        left++;
        var x = grid[row][i];
        if (x >= value)
        {
            break;
        }
    }

    var right = 0;
    for (var i = column + 1; i <= size; i++)
    {
        right++;
        var x = grid[row][i];
        if (x >= value)
        {
            break;
        }
    }

    var up = 0;
    for (var i = row - 1; i >= 0; i--)
    {
        up++;
        var x = grid[i][column];
        if (x >= value)
        {
            break;
        }
    }

    var down = 0;
    for (var i = row + 1; i <= size; i++)
    {
        down++;
        var x = grid[i][column];
        if (x >= value)
        {
            break;
        }
    }

    return (up, down, left, right);
}
