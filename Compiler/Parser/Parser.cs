using System.Data;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Reflection.Metadata.Ecma335;
using Compiler.Enum;
using Compiler.Extension;
using Compiler.Interface;
using Compiler.Language;
using Compiler.Tokenizador;
using ValueType = Compiler.Language.ValueType;

namespace Compiler.Parser;

public class Parser
{
    private int tokenIndex;
    public delegate bool GetExpressionMethod(Token[] tokens, out IExpression? exp);

    public IInstruction Parse(Token[] tokens)
    {
        tokenIndex = 0;
        if (!GetBlock(tokens, out IInstruction block))
            throw new Exception();
        if (!MatchToken(tokens, TokenType.EndOfFile))
            throw new Exception();
        return block;
    }

    private bool GetBlock(Token[] tokens, out IInstruction exp)
    {
        List<IInstruction> lines = [];
        bool move;
        do
        {
            if (move = GetAssign(tokens, out IInstruction? line))
                lines.Add(line!);
            else if (move = GetCallableAction(tokens, out line))
                lines.Add(line!);
            else if (move = GetGoTo(tokens, out line))
                lines.Add(line!);
            else if (move = GetLabel(tokens, out line))
            {
                lines.Add(line!);
                continue;
            }

            MatchToken(tokens, TokenType.EndLine);
        } while (move);

        var node = new BlockExpression(lines);
        return SetExp(node, out exp);
    }
    private bool GetGoTo(Token[] tokens, out IInstruction? line)
    {
        var startIndex = tokenIndex;
        if (!MatchToken(tokens, TokenType.GoTo)
            || !MatchToken(tokens, TokenType.CorcheteIzquierdo)
            || !MatchToken(tokens, TokenType.Identificador)
        )
            return ResetExp(startIndex, out line);

        var name = tokens[tokenIndex - 1].Value;
        if (!MatchToken(tokens, TokenType.CorcheteDerecho)
            || !MatchToken(tokens, TokenType.ParentesisIzquierdo)
            || !GetBoolExpression(tokens, out IExpression? cond)
            || !MatchToken(tokens, TokenType.ParentesisDerecho))
            return ResetExp(startIndex, out line);

        var nodo = new GotoInstruction(name, cond!);
        return SetExp(nodo, out line);
    }

    private bool GetLabel(Token[] tokens, out IInstruction? line)
    {
        var startIndex = tokenIndex;
        if (!MatchToken(tokens, TokenType.Label))
            return ResetExp(startIndex, out line);
        var token = tokens[tokenIndex - 1];
        var node = new LabelInstruction(token.Value, token.Row);
        return SetExp(node, out line);
    }

    private bool GetAssign(Token[] tokens, out IInstruction? line)
    {
        var startIndex = tokenIndex;
        var name = tokens[tokenIndex].Value;
        if (!MatchToken(tokens, TokenType.Identificador))
            return ResetExp(startIndex, out line);

        if (!MatchToken(tokens, TokenType.Assign))
            return ResetExp(startIndex, out line);

        if (!GetExpression(tokens, out IExpression? value))
            return ResetExp(startIndex, out line);

        var node = new Assign(name, value!);
        return SetExp(node, out line);
    }

    private bool GetExpression(Token[] tokens, out IExpression? value)
    {
        int startIndex = tokenIndex;
        if (GetNumExpression(tokens, out IExpression? num))
            return SetExp(num, out value);
        return ResetExp(startIndex, out value);
    }

    private bool GetCallableAction(Token[] tokens, out IInstruction? line)
    {
        var startIndex = tokenIndex;
        var name = tokens[tokenIndex].Value;

        if (!MatchToken(tokens, TokenType.Identificador))
            return ResetExp(startIndex, out line);

        if (!MatchToken(tokens, TokenType.ParentesisIzquierdo))
            return ResetExp(startIndex, out line);

        if (!GetParams(tokens, out IExpression[]? @params))
            return ResetExp(startIndex, out line);

        var newNode = new CallableAction(name, @params!);
        return SetExp(newNode, out line);
    }

    private bool GetCallableFunc(Token[] tokens, out IExpression? exp)
    {
        var startIndex = tokenIndex;
        var name = tokens[tokenIndex].Value;

        if (!MatchToken(tokens, TokenType.Identificador))
            return ResetExp(startIndex, out exp);

        if (!MatchToken(tokens, TokenType.ParentesisIzquierdo))
            return ResetExp(startIndex, out exp);

        if (!GetParams(tokens, out IExpression[]? @params))
            return ResetExp(startIndex, out exp);

        var newNode = new CallableFunc(name, @params!);
        return SetExp(newNode, out exp);
    }

    private bool GetParams(Token[] tokens, out IExpression[]? @params)
    {
        var startIndex = tokenIndex;
        List<IExpression> parameters = [];
        if (!MatchToken(tokens, TokenType.ParentesisDerecho) && GetExpression(tokens, out IExpression? value))
            parameters.Add(value!);
        while (!MatchToken(tokens, TokenType.ParentesisDerecho))
        {
            if (!MatchToken(tokens, TokenType.Coma) || !GetExpression(tokens, out value))
                return ResetExp(startIndex, out @params);
            parameters.Add(value!);
        }

        @params = parameters.ToArray();
        return true;

    }

    #region Arithmetic

    private bool GetNumExpression(Token[] tokens, out IExpression? num)
        => GetNumSumExpression(tokens, out num);
    private bool GetNumSumExpression(Token[] tokens, out IExpression? num)
        => ParseExpression(tokens, out num, GetMultExpression, [TokenType.Suma, TokenType.Resta]);
    private bool GetMultExpression(Token[] tokens, out IExpression? num)
        => ParseExpression(tokens, out num, GetPowExpression, [TokenType.Mult, TokenType.Div, TokenType.Module]);
    private bool GetPowExpression(Token[] tokens, out IExpression? num)
        => ParseExpression(tokens, out num, GetNumLiteral, [TokenType.Pow]);
    private bool GetNumLiteral(Token[] tokens, out IExpression? exp)
        => GetLiteralExp(tokens, out exp, GetNumExpression, TokenType.Num);

    #endregion

    #region Boolean 

    private bool GetBoolExpression(Token[] tokens, out IExpression? cond)
        // => GetComparisonExpression(tokens, out cond);
        => GetOrExpression(tokens, out cond);
    private bool GetOrExpression(Token[] tokens, out IExpression? cond)
        => ParseExpression(tokens, out cond, GetAndExpression, [TokenType.Or]);
    private bool GetAndExpression(Token[] tokens, out IExpression? cond)
        => ParseExpression(tokens, out cond, GetComparisonExpression, [TokenType.And]);
    private bool GetBoolLiteral(Token[] tokens, out IExpression? cond)
        => ParseExpression(tokens, out cond, GetBoolLiteral, [TokenType.And]);



    // Implemetar los comparadores, son reduce porque compara primero y lo convierte a booleano : listo, revisar bien

    #endregion

    #region Comparadores
    private bool GetComparisonExpression(Token[] tokens, out IExpression? cond)
    => ParseExpression(tokens, out cond, GetNumExpression, [TokenType.Mayor, TokenType.Menor, TokenType.MayorIgual, TokenType.MenorIgual, TokenType.Igual, TokenType.Diferente]);

    #endregion

    #region Tools

    private bool ParseExpression(Token[] tokens, out IExpression? expre, GetExpressionMethod Parse, TokenType[] types)
    {
        int startIndex = tokenIndex;
        if (!Parse(tokens, out IExpression? left))
            return ResetExp(startIndex, out expre);
        return ConstructionExpression(tokens, out expre, left!, Parse, types);
    }

    private bool ConstructionExpression(Token[] tokens, out IExpression? expre, IExpression left, GetExpressionMethod Parse, TokenType[] types)
    {
        int startIndex = tokenIndex;
        if (!GetMatchingTokenType(tokens, types, out TokenType type))
            return SetExp(left, out expre);
        var binary = type.GetBinaryType();
        var isShift = binary.IsShift();
        var result = isShift
            ? ShiftExpression(tokens, out expre, left, Parse, binary, types)
            : ReduceExpression(tokens, out expre, left, Parse, binary, types);
        return result || ResetExp(startIndex, out expre);
    }

    private bool ReduceExpression(Token[] tokens, out IExpression? expre, IExpression left, GetExpressionMethod Parse, BinaryType binary, TokenType[] types)
    {
        int startIndex = tokenIndex;
        if (!Parse(tokens, out IExpression? right))
            return ResetExp(startIndex, out expre);
        var node = new BinaryExpression(left!, right!, binary);
        return ConstructionExpression(tokens, out expre, node, Parse, types);
    }

    private bool ShiftExpression(Token[] tokens, out IExpression? expre, IExpression left, GetExpressionMethod Parse, BinaryType binary, TokenType[] types)
    {
        int startIndex = tokenIndex;
        if (!ParseExpression(tokens, out IExpression? right, Parse, types))
            return ResetExp(startIndex, out expre);
        var node = new BinaryExpression(left!, right!, binary);
        return SetExp(node, out expre);
    }

    private bool GetMatchingTokenType(Token[] tokens, TokenType[] types, out TokenType type)
    {
        foreach (var item in types)
        {
            if (MatchToken(tokens, item))
                return SetExp(item, out type);
        }
        return ResetExp(tokenIndex, out type);
    }

    private bool GetLiteralExp(Token[] tokens, out IExpression? exp, GetExpressionMethod Parse, TokenType type)
    {
        var token = tokens[tokenIndex];
        if (MatchToken(tokens, type) && ValueType.TryParse(token.Value, type, out ValueType? literal))
            return SetExp(new Literal(literal!), out exp);
        if (MatchToken(tokens, TokenType.ParentesisIzquierdo))
        {
            return Parse(tokens, out IExpression? value) && MatchToken(tokens, TokenType.ParentesisDerecho)
                ? SetExp(value!, out exp)
                : throw new InvalidExpressionException();
        }
        if (GetCallableFunc(tokens, out IExpression? func))
            return SetExp(func!, out exp);
        if (MatchToken(tokens, TokenType.Identificador))
            return SetExp(new Variable(token.Value), out exp);
        throw new InvalidExpressionException();
    }

    private bool MatchToken(Token[] tokens, TokenType type)
    {
        if (tokens[tokenIndex].Type != type)
            return false;
        tokenIndex++;
        return true;
    }

    private bool ResetExp<T>(int startIndex, out T? line)
    {
        tokenIndex = startIndex;
        line = default;
        return false;
    }

    private bool SetExp<T>(T value, out T result)
    {
        result = value;
        return true;
    }

    #endregion
}
