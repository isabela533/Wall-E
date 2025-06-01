
using Compiler.Interface;
using Compiler.Model;

class GotoInstruction(string target, IExpression cond) : IInstruction
{
    public string Target { get; } = target;
    public IExpression Cond { get; } = cond;

    public void Accept(Context context)
    {
        if (Cond.Accept(context).Value is not bool cond || !cond)
            return;
        context.JumpTo(Target);
    }
}

class LabelInstruction(string name, int row) : IInstruction
{
    public string Name { get; } = name;
    public int Row { get; } = row;

    public void Accept(Context context) => context.Labels[Name] = Row;
}