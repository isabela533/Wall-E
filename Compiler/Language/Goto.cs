
using System.Data;
using Compiler.Interface;
using Compiler.Language;
using Compiler.Model;

class GotoInstruction(string target, IExpression cond, int row) : IInstruction
{
    public string Target { get; } = target;
    public IExpression Cond { get; } = cond;
    public int Row { get; } = row;
    public int LabelRow { get; private set; }

    public void Accept(Context context)
    {
        // TODO: cambiar a ingles
        if (!context.Labels.TryGetValue(Target, out var labelRow))
            throw new KeyNotFoundException($"No existe etiqueta de nombre {Target}");
        var value = Cond.Accept(context);
        if (value.Value is not bool cond)
            throw new InvalidOperationException($"Se esperaba un Boolean y se recibio un {value.Kind}");
        if (!cond && labelRow < Row || !cond && labelRow > Row)
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