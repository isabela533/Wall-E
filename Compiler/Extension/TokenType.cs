using Compiler.Enum;
using Compiler.Tokenizador;

namespace Compiler.Extension;

public static class TokenExtension
{
    public static BinaryType GetBinaryType(this TokenType type) => type switch
    {
        TokenType.Suma => BinaryType.Addition,
        TokenType.Mult => BinaryType.Multiplication,
        TokenType.Pow => BinaryType.Potencia,
        TokenType.Resta => BinaryType.Diferencia,
        TokenType.Div => BinaryType.Division,
        _ => throw new Exception(),
    };
}