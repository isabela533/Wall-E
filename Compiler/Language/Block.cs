using Compiler.Interface;
using Compiler.Model;

namespace Compiler.Language;

internal class BlockExpression(List<IInstruction> lines) : IInstruction
{
    private readonly List<IInstruction> Lines = lines;

    public void Accept(Context context)
    {
        // TODO: Hacer un método que ejecute solo los labels, de esta forma el goto puede ver un label que este posterior a el.
        for (int i = 0; i < Lines.Count; i++)
        {
            Lines[i].Accept(context);
            if(context.Jump)
            {
                i = context.Index;
                context.ResetJump();
            }
        }
    }
}