using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Numerics;
using Compiler.Tokenizador;

namespace Compiler.Language;

public class ValueType(TokenType kind, object value)
{
    public TokenType Kind { get; } = kind;
    public object Value { get; } = value;

    #region Arithmetics
    public static ValueType operator +(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");

        return new ValueType(TokenType.Num, (int)a.Value + (int)b.Value);
    }

    public static ValueType operator *(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");

        return new ValueType(TokenType.Num, (int)a.Value * (int)b.Value);
    }

    public static ValueType Pow(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Num, (int)Math.Pow((int)a.Value, (int)b.Value));
    }

    public static ValueType operator /(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        if (b.Value.Equals(0))
            throw new DivideByZeroException();
        return new ValueType(TokenType.Num, (int)a.Value / (int)b.Value);
    }

    public static ValueType operator %(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        if (b.Value.Equals(0))
            throw new DivideByZeroException();
        return new ValueType(TokenType.Num, (int)a.Value % (int)b.Value);
    }


    public static ValueType operator -(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Num, (int)a.Value - (int)b.Value);
    }

    #endregion

    #region Boolean & Unary 
    public static ValueType operator -(ValueType a)
    {
        if (a.Kind != TokenType.Num)
            throw new InvalidOperationException("Unary '-' only supported on numeric types");

        return new ValueType(TokenType.Num, -1 * (int)a.Value);
    }

    public static ValueType operator +(ValueType a)
    {
        if (a.Kind != TokenType.Num)
            throw new InvalidOperationException("Unary '+' only supported on numeric types");

        return new ValueType(TokenType.Num, +1 * (int)a.Value);
    }
    
    public static ValueType operator !(ValueType a)
    {
        if (a.Kind != TokenType.Boolean)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Boolean, !(bool)a.Value);
    }

    public static ValueType operator ==(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Boolean, Equals(a.Value, b.Value));
    }

    public static ValueType operator !=(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Boolean, !Equals(a.Value,b.Value));
    }

    public static ValueType operator >(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Boolean, (int)a.Value > (int)b.Value);
    }

    public static ValueType operator <(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Boolean, (int)a.Value < (int)b.Value);
    }

    public static ValueType operator >=(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Boolean, (int)a.Value >= (int)b.Value);
    }

    public static ValueType operator <=(ValueType a, ValueType b)
    {
        if (a.Kind != b.Kind)
            throw new InvalidOperationException("Unsupported operation for the given types");
        return new ValueType(TokenType.Boolean, (int)a.Value <= (int)b.Value);
    }
    #endregion 
    public static ValueType Parse(string s, TokenType type)
    {
        return type switch
        {
            TokenType.Boolean => new ValueType(TokenType.Boolean, bool.Parse(s)),
            TokenType.Num => new ValueType(TokenType.Num, int.Parse(s)),
            TokenType.String => new ValueType(TokenType.String, s),
            _ => throw new NotImplementedException(),
        };
    }

    public static bool TryParse(string? s, TokenType type, out ValueType? result)
    {
        result = null;
        object? value = type switch
        {
            TokenType.Boolean => bool.TryParse(s, out bool @bool) ? @bool : null,
            TokenType.Num => int.TryParse(s, out int num) ? num : null,
            TokenType.String => s,
            _ => null
        };
        if (value is null)
            return false;
        result = new ValueType(type, value);
        return true;
    }

    public override bool Equals(object? obj)
        => ReferenceEquals(obj, this) || (obj is not null && obj is ValueType that && that.Value == Value);

    public override int GetHashCode() => Value.GetHashCode();
}
