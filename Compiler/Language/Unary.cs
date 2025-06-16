using Compiler.Enum;
using Compiler.Interface;
using Compiler.Model;

namespace Compiler.Language;

public class UnaryExpression(IExpression expression, UnaryType type) : IExpression
{
    public IExpression Expression => expression;
    public UnaryType Type => type;

    public ValueType Accept(Context context)
    {
        ValueType value = expression.Accept(context);
        return type switch
        {
            UnaryType.Not => !value,
            UnaryType.Negative => - value,
            UnaryType.Positive => + value,
            _ => throw new InvalidOperationException(),
        };
    }
}
