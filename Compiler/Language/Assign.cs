using Compiler.Interface;
using Compiler.Model;

namespace Compiler.Language;

public class Assign(string name, IExpression value) : IInstruction
{
    public string Name => name;
    public IExpression Value => value;

    public void Accept(Context context) => context.Variables[Name] = Value.Accept(context);
}