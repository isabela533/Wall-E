using Compiler.Extension;
using Compiler.Interface;
using Compiler.Model;
using Compiler.Tokenizador;

namespace Compiler.Language;

public class Variable(string name) : IExpression
{
    public string Name => name;

    public ValueType Accept(Context context)
    {
        return context.Variables[Name];
    }
}