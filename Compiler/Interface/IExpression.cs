using Compiler.Model;
using ValueType = Compiler.Language.ValueType;

namespace Compiler.Interface;

public interface IExpression
{
    public ValueType Accept(Context context);
}