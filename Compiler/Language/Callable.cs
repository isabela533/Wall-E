using Compiler.Interface;
using Compiler.Model;

namespace Compiler.Language;

public class CallableFunc(string name, IExpression[] parameters) : IInstruction, IExpression
{
    public string Name => name;
    public IExpression[] Params => parameters;

    public ValueType Accept(Context context)
    {
        throw new NotImplementedException();
    }

    void IInstruction.Accept(Context context) => Accept(context);
}

public class CallableAction(string name, IExpression[] parameters) : IInstruction
{
    public string Name => name;
    public IExpression[] Params => parameters;

    public void Accept(Context context)
    {
        throw new NotImplementedException();
    }

}
