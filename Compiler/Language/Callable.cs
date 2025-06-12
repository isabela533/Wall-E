using System.Reflection.Metadata.Ecma335;
using Compiler.Interface;
using Compiler.Model;

namespace Compiler.Language;

public class CallableFunc(string name, IExpression[] parameters) : IInstruction, IExpression
{
    public string Name => name;
    public IExpression[] Params => parameters;

    public ValueType Accept(Context context)
    {
        ValueType[] paramValues = [.. Params.Select(x => x.Accept(context))];
        return context.CallFunction(Name, paramValues);
    }

    void IInstruction.Accept(Context context) => Accept(context);
}

public class CallableAction(string name, IExpression[] parameters) : IInstruction
{
    public string Name => name;
    public IExpression[] Params => parameters;

    public void Accept(Context context)
    {
        ValueType[] paramValues = [.. Params.Select(x => x.Accept(context))];
        context.ExecuteAction(Name, paramValues);
    }

}
