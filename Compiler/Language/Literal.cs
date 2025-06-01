using Compiler.Interface;
using Compiler.Model;

namespace Compiler.Language;

public class Literal(ValueType value) : IExpression
{
    public ValueType Value => value;
    public ValueType Accept(Context context) => value;
}