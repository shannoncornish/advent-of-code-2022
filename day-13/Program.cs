var inputPath = $"{args.FirstOrDefault() ?? "Input"}.txt";
var input = File.ReadAllText(inputPath).Split("\n\n");

var comparer = new ListOrIntegerComparer();

var pairs = input.Select((pair, i) =>
{
    var packets = pair.Split("\n");
    return new
    {
        Index = i + 1,
        Left = ParsePacket(packets[0]),
        Right = ParsePacket(packets[1]),
    };
});

var sum = pairs.Where(pair => comparer.Compare(pair.Left, pair.Right) == -1).Sum(pair => pair.Index);
Console.WriteLine($"Sum of indicies in right order: {sum}"); // 4809

var packets = pairs.SelectMany(pair => new[] { pair.Left, pair.Right }).ToList();

// Distress protocol devider packets.
var firstDividerPacket = ParsePacket("[[2]]");
packets.Add(firstDividerPacket);
var secondDividerPacket = ParsePacket("[[6]]");
packets.Add(secondDividerPacket);

var ordered = packets.OrderBy(packet => packet, comparer).ToList();

var firstDividerIndex = ordered.IndexOf(firstDividerPacket) + 1;
var secondDividerIndex = ordered.IndexOf(secondDividerPacket, firstDividerIndex) + 1;

Console.WriteLine($"Decoder key for the distress signal: {firstDividerIndex * secondDividerIndex}"); // 22600

ListOrInteger ParsePacket(string packet)
{
    var root = new ListOrInteger(new List<ListOrInteger>());

    var stack = new Stack<ListOrInteger>();
    stack.Push(root);

    var tokens = ParseTokens(packet);
    foreach (var token in tokens)
    {
        switch (token.Type)
        {
            case TokenType.StartList:
                var list = new ListOrInteger(new List<ListOrInteger>());
                stack.Peek().List!.Add(list);
                stack.Push(list);
                break;
            case TokenType.Integer:
                var integer = new ListOrInteger(token.Value);
                stack.Peek().List!.Add(integer);
                break;
            case TokenType.EndList:
                stack.Pop();
                break;
        }
    }

    return root.List![0];
}

IEnumerable<Token> ParseTokens(string packet)
{
    var numbers = "";
    for (var i = 0; i < packet.Length; i++)
    {
        var c = packet[i];
        if (c is '[')
        {
            yield return new Token(TokenType.StartList, 0);
            continue;
        }

        if (c is >= '0' and <= '9')
        {
            numbers += c;
        }

        if (c is ',' or ']' && numbers is not "")
        {
            var value = int.Parse(numbers);
            numbers = "";

            yield return new Token(TokenType.Integer, value);
        }

        if (c is ']')
        {
            yield return new Token(TokenType.EndList, 0);
        }
    }
}

record struct Token(TokenType Type, int Value);

enum TokenType { StartList, EndList, Integer, }

record struct ListOrInteger(List<ListOrInteger>? List, int? Integer)
{
    public ListOrInteger(List<ListOrInteger> list) : this(list, null) { }

    public ListOrInteger(int integer) : this(null, integer) { }

    public ListOrInteger AsList()
    {
        if (List is not null)
        {
            return this;
        }

        return new ListOrInteger(new List<ListOrInteger> { this });
    }
}

class ListOrIntegerComparer : IComparer<ListOrInteger>
{
    public int Compare(ListOrInteger left, ListOrInteger right)
    {
        if (left.Integer is not null && right.Integer is not null)
        {
            return left.Integer.Value.CompareTo(right.Integer.Value);
        }

        var leftEnumerator = left.AsList().List!.GetEnumerator();
        var rightEnumerator = right.AsList().List!.GetEnumerator();

        int itemComparison = 0;
        do
        {
            var leftHasCurrent = leftEnumerator.MoveNext();
            var rightHasCurrent = rightEnumerator.MoveNext();

            if (!leftHasCurrent && !rightHasCurrent)
            {
                return 0;
            }

            if (!leftHasCurrent)
            {
                return -1;
            }

            if (!rightHasCurrent)
            {
                return 1;
            }

        } while ((itemComparison = Compare(leftEnumerator.Current, rightEnumerator.Current)) is 0);

        return itemComparison;
    }
}
