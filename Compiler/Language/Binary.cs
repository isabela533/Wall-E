using Compiler.Enum;
using Compiler.Interface;
using Compiler.Model;

namespace Compiler.Language;

public class BinaryExpression(IExpression left, IExpression right, BinaryType type) : IExpression
{
    public IExpression Left => left;
    public IExpression Right => right;
    public BinaryType Type => type;
    public ValueType Accept(Context context)
    {
        return type switch
        {
            BinaryType.Addition => left.Accept(context) + right.Accept(context),
            BinaryType.Diferencia => left.Accept(context) - right.Accept(context),
            BinaryType.Multiplication => left.Accept(context) * right.Accept(context),
            BinaryType.Potencia => ValueType.Pow(left.Accept(context), right.Accept(context)),
            BinaryType.Division => left.Accept(context) / right.Accept(context),
            BinaryType.Module => left.Accept(context) % right.Accept(context),
            BinaryType.Igual => left.Accept(context) == right.Accept(context),
            BinaryType.Distinto => left.Accept(context) != right.Accept(context),
            BinaryType.MayorQue => left.Accept(context) > right.Accept(context),
            BinaryType.MenorQue => left.Accept(context) < right.Accept(context),
            BinaryType.MayorIgual => left.Accept(context) >= right.Accept(context),
            BinaryType.MenorIgual => left.Accept(context) <= right.Accept(context),
            _ => throw new InvalidOperationException(),
        };
    }
}
