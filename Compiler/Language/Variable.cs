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
        // TODO: cambiar a ingles
        if (!context.Variables.TryGetValue(Name, out var value))
            throw new KeyNotFoundException($"No existe variable de nombre {Name}");
        return value;
    }
}