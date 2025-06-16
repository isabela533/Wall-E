using System.Runtime;
using Compiler.Language;
using ValueType = Compiler.Language.ValueType;

namespace Compiler.Model;

public class Context(IContextCallable? callable = null) : IContextCallable
{
    public Dictionary<string, ValueType> Variables { get; protected set; } = [];
    public Dictionary<string, int> Labels { get; protected set; } = [];

    public string? Target { get; protected set; }
    public bool Jump { get; protected set; }
    public int Index => Labels[Target!];

    public void JumpTo(string target)
    {
        Target = target;
        Jump = true;
    }

    public void ResetJump()
    {
        Target = null;
        Jump = false;
    }

    public ValueType CallFunction(string name, ValueType[] paramValues)
        => callable?.CallFunction(name, paramValues) ??
        throw new NotImplementedException();

    public void ExecuteAction(string name, ValueType[] paramValues)
        => callable?.ExecuteAction(name, paramValues);

    public bool CheckParamsType(string name, ValueType[] paramValues)
        => callable?.CheckParamsType(name, paramValues) ??
        throw new NotImplementedException();

    public bool CheckParamsCount(string name, ValueType [] paramValues, out int cantParams)
        => callable?.CheckParamsCount(name, paramValues, out cantParams) ??
        throw new NotImplementedException();
}

public interface IContextCallable
{
    ValueType CallFunction(string name, ValueType[] paramValues);
    void ExecuteAction(string name, ValueType[] paramValues);
    bool CheckParamsType(string name, ValueType[] paramValues);
    bool CheckParamsCount(string name, ValueType[] paramValues, out int cantParams);
}