namespace Compiler.Tokenizador;

public class Token
{
    public string Value { get; set; }

    public TokenType Type { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }

    public Token(string value, TokenType type, int row = 0, int col = 0)
    {
        Value = type is not TokenType.Label ?
                value : value[^2] == '\r' ?
                value[..^2] : value[..^1];
        Type = type;
        Row = row;
        Col = col;
    }

    public override string ToString()
    {
        var str = Value[^1] == '\n' ? "\\n" : Value;
        return $"({str}, {Type}) -> {Row}:{Col}";
    }
}
