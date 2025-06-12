using Compiler.Tokenizador;

namespace Compiler.Extension;

public static class ValueKindExtension
{
    public static Type GetValueKind(this ValueType value)
    {
        return value switch
        {
            TokenType.Num => typeof(int),
            TokenType.Boolean => typeof(bool),
            TokenType.String => typeof(string),
            _ => throw new NotSupportedException("Type of value not supported"),
        };
    }
}