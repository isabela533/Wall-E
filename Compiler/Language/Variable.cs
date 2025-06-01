using Compiler.Interface;
using Compiler.Model;
using Compiler.Tokenizador;

namespace Compiler.Language;

public class Variable(string name) : IExpression
{
    public string Name => name;

    public ValueType Accept(Context context)
    {
        // TODO: instalar todo tree
        // TODO: cambiar el kind por uno correcto (hacer metodo extensor a partir del type de value) -> typeof(value)
        return context.Variables[Name];
    }
}