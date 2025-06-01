using System.Runtime;
using ValueType = Compiler.Language.ValueType;

namespace Compiler.Model;

public class Context
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
}