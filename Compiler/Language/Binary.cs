using Compiler.Enum;
using Compiler.Interface;
using Compiler.Model;
using Compiler.Tokenizador;

namespace Compiler.Language;

public class BinaryExpression(IExpression left, IExpression right, BinaryType type) : IExpression
{
    public IExpression Left => left;
    public IExpression Right => right;
    public BinaryType Type => type;
    public ValueType Accept(Context context)
    {
        var left = Left.Accept(context);
        var right = Right.Accept(context);
        // TODO: cambiar a ingles
        if (left.Kind != right.Kind)
            throw new InvalidOperationException("Tienen que ser del mismo tipo");
        if (left.Kind != TokenType.Num && type is not BinaryType.Igual or BinaryType.Distinto)
            throw new InvalidOperationException($"No se admite {type} entre {left.Kind} y {right.Kind}");
        return type switch
        {
            BinaryType.Addition => left + right,
            BinaryType.Diferencia => left - right,
            BinaryType.Multiplication => left * right,
            BinaryType.Potencia => ValueType.Pow(left, right),
            BinaryType.Division => left / right,
            BinaryType.Module => left % right,
            BinaryType.Igual => left == right,
            BinaryType.Distinto => left != right,
            BinaryType.MayorQue => left > right,
            BinaryType.MenorQue => left < right,
            BinaryType.MayorIgual => left >= right,
            BinaryType.MenorIgual => left <= right,
            _ => throw new InvalidOperationException(),
        };
    }
}
