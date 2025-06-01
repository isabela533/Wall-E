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
            BinaryType.Multiplication => throw new NotImplementedException(),
            BinaryType.Potencia => throw new NotImplementedException(),
            BinaryType.Division => throw new NotImplementedException(),
            _ => throw new InvalidOperationException(),
        };
    }
}
