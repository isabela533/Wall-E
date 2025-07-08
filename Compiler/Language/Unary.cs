using Compiler.Enum;
using Compiler.Interface;
using Compiler.Model;
using Compiler.Tokenizador;

namespace Compiler.Language;

public class UnaryExpression(IExpression expression, UnaryType type) : IExpression
{
    public IExpression Expression => expression;
    public UnaryType Type => type;

    public ValueType Accept(Context context)
    {
        ValueType value = expression.Accept(context);
        // TODO: cambiar a ingles
        if (type is UnaryType.Not && value.Kind != TokenType.Boolean
        || type is not UnaryType.Not && value.Kind != TokenType.Num)
            throw new InvalidOperationException($"No se admite {type} con {value.Kind}");
        return type switch
        {
            UnaryType.Not => !value,
            UnaryType.Negative => -value,
            UnaryType.Positive => +value,
            _ => throw new InvalidOperationException(),
        };
    }
}
