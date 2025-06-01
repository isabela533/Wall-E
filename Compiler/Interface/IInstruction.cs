using Compiler.Model;

namespace Compiler.Interface;

public interface IInstruction
{
    void Accept(Context context);
}
