using System.Text.RegularExpressions;

namespace Compiler.Tokenizador;

public class Tokenizador
{
    #region Patterns

    public static readonly string id = @"[a-zA-Z][a-zA-Z0-9_]*";
    public static readonly string label = $@"({id})(\r?\n)";
    public static readonly string num = $@"-?\d+|{id}-\d+"; //lo arregle, ver si funciona , lo debuggee y me da bien, de todas formas seguir probando 
    public static readonly string str = @"""[^""]*""";
    public static readonly string otherOp = @"\*\*";
    public static readonly string comp = @"[<>=\!]=";
    public static readonly string assign = @"<-";
    public static readonly string op = @"[<>%+\-\*/&\|\!\(\),\[\]]";
    public static readonly string efl = @"\n|\r\n";

    #endregion

    public List<string> errores = new List<string>();
    public static string GetAllPatterns()
    {
        return string.Join('|', @"[\t ]+", str, label, id, num, assign, otherOp, comp, op, efl);
    }

    public Token[] Tokenizar(string code)
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
            {
                errores.Add($"Error processing code: expected a valid token at line {line + 1}, column {characters + 1}. Please check that the code is correctly formatted and that there are no syntax errors.");
                continue;
            }
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

        if (Regex.IsMatch(value, label))
            return TokenType.Label;
        if (value[^1] == '"' && value[0] == '"')
            return TokenType.String;
        if (bool.TryParse(value, out bool _))
            return TokenType.Boolean;
        if (Regex.IsMatch(value, num))
            return TokenType.Num;
        return TokenType.Identificador;
    }

    public List<string> GetErrores()
    {
        return errores;
    }
}