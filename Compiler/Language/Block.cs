using System.Reflection.Emit;
using Compiler.Interface;
using Compiler.Model;
using Compiler.Tokenizador;

namespace Compiler.Language;

internal class BlockExpression(List<IInstruction> lines) : IInstruction
{
    //TODO: se parte aqui 
    private readonly List<IInstruction> Lines = lines;

    public void Accept(Context context)
    {
        EjectutarLabel(context);
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

    private void EjectutarLabel(Context context)
    {
        foreach(var line in Lines)
        {
            if (line is LabelInstruction label)
                label.Accept(context);
        }
    }
}