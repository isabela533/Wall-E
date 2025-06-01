using Compiler.Enum;

namespace Compiler.Extension;

public static class BinaryTypeExtension
{
    public static bool IsShift(this BinaryType binary) => binary switch
    {
        BinaryType.Addition or BinaryType.Potencia or BinaryType.Multiplication => true,
        BinaryType.Diferencia or BinaryType.Division => false,
        _ => throw new InvalidCastException(),
    };
}