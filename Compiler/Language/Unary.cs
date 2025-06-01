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
        return type switch
        {
            UnaryType.Negative => !expression.Accept(context),
            _ => throw new InvalidOperationException(),
        };
    }
}
