using Compiler.Enum;
using Compiler.Tokenizador;

namespace Compiler.Extension;

public static class TokenExtension
{
    public static BinaryType GetBinaryType(this TokenType type) => type switch
    {
        TokenType.Suma => BinaryType.Addition,
        TokenType.Mult => BinaryType.Multiplication,
        TokenType.Module => BinaryType.Module,
        TokenType.Pow => BinaryType.Potencia,
        TokenType.Resta => BinaryType.Diferencia,
        TokenType.Div => BinaryType.Division,
        TokenType.Menor => BinaryType.MenorQue,
        TokenType.Mayor => BinaryType.MayorQue,
        TokenType.MenorIgual => BinaryType.MenorIgual,
        TokenType.MayorIgual => BinaryType.MayorIgual,
        TokenType.Igual => BinaryType.Igual,
        TokenType.Diferente => BinaryType.Distinto,
        _ => throw new Exception(),
    };
}