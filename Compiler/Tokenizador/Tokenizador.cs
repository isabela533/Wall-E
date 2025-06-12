using System.Text.RegularExpressions;

namespace Compiler.Tokenizador;

// TODO: Hacer clase no estatica y añadir lista de errores
public static class Tokenizador
{
    #region Patterns

    public static readonly string id = @"[a-zA-Z][a-zA-Z0-9_]*";
    public static readonly string label = $@"{id}\n|{id}\r\n";
    public static readonly string num = @"\d+,\d+|\d+";
    public static readonly string str = @"""[^""]*""";
    public static readonly string otherOp = @"\*\*";
    public static readonly string comp = @"[<>=\!]=";
    public static readonly string assign = @"<-";
    public static readonly string op = @"[<>%+\-\*/&\|\!\(\),\[\]]";
    public static readonly string efl = @"\n|\r\n";

    #endregion

    public static string GetAllPatterns()
    {
        return string.Join('|', @"[\t ]+", str, label, id, num, assign, otherOp, comp, op, efl);
    }

    public static Token[] Tokenizar(string code)
    {
        var pattern = GetAllPatterns();
        MatchCollection matches = Regex.Matches(code, pattern);

        List<Token> tokens = [];
        int characters = 0;
        int index = 0;
        int line = 0;
        foreach (Match match in matches.Cast<Match>())
        {
            if (match.Index != index)
                throw new Exception();
            var value = match.Value;
            var type = GetTokenType(value);
            if (value[0] != ' ')
                tokens.Add(new Token(value, type, line, characters));
            characters += match.Length;
            index += match.Length;
            if (type != TokenType.EndLine)
                continue;
            line++;
            characters = 0;
        }

        tokens.Add(new Token("$", TokenType.EndOfFile, line + 1, 0));
        return [.. tokens];
    }

    private static TokenType GetTokenType(string value)
    {
        switch (value)
        {
            case "+":
                return TokenType.Suma;
            case "-":
                return TokenType.Resta;
            case "/":
                return TokenType.Div;
            case "*":
                return TokenType.Mult;
            case "%":
                return TokenType.Module;
            case "**":
                return TokenType.Pow;
            case "&":
                return TokenType.And;
            case "<-":
                return TokenType.Assign;
            case "\n":
            case "\r\n":
                return TokenType.EndLine;
            case "<":
                return TokenType.Menor;
            case ">":
                return TokenType.Mayor;
            case "<=":
                return TokenType.MenorIgual;
            case ">=":
                return TokenType.MayorIgual;
            case "==":
                return TokenType.Igual;
            case "!=":
                return TokenType.Diferente;
            case "!":
                return TokenType.Negacion;
            case "(":
                return TokenType.ParentesisIzquierdo;
            case ")":
                return TokenType.ParentesisDerecho;
            case ",":
                return TokenType.Coma;
            case ":":
                return TokenType.DosPuntos;
            case "[":
                return TokenType.CorcheteIzquierdo;
            case "]":
                return TokenType.CorcheteDerecho;
            case "GoTo":
                return TokenType.GoTo;
        }

        if (value[^1] == '\n')
            return TokenType.Label;
        if (value[^1] == '"' && value[0] == '"')
            return TokenType.String;
        if (bool.TryParse(value, out bool _))
            return TokenType.Boolean;
        if (int.TryParse(value, out int _))
            return TokenType.Num;
        return TokenType.Identificador;
    }
}