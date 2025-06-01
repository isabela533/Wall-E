using System.Data;
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
        if (tokens[tokenIndex].Type != TokenType.EndOfFile)
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
            else if (move = GetCallable(tokens, out line))
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
        if (tokens[tokenIndex++].Type != TokenType.GoTo
            || tokens[tokenIndex++].Type != TokenType.CorcheteIzquierdo
            || tokens[tokenIndex++].Type != TokenType.Identificador
        )
            return ResetExp(startIndex, out line);

        var name = tokens[tokenIndex - 1].Value;
        if (tokens[tokenIndex++].Type != TokenType.CorcheteDerecho
            || tokens[tokenIndex++].Type != TokenType.ParentesisIzquierdo
            || !GetBoolExpression(tokens, out IExpression? cond)
            || tokens[tokenIndex++].Type != TokenType.ParentesisDerecho)
            return ResetExp(startIndex, out line);

        var nodo = new GotoInstruction(name, cond!);
        return SetExp(nodo, out line);
    }

    private bool GetLabel(Token[] tokens, out IInstruction? line)
    {
        var startIndex = tokenIndex;
        if (tokens[tokenIndex++].Type != TokenType.Label)
            return ResetExp(startIndex, out line);
        var token = tokens[tokenIndex - 1];
        var node = new LabelInstruction(token.Value, token.Row);
        return SetExp(node, out line);
    }

    private bool GetAssign(Token[] tokens, out IInstruction? line)
    {
        var startIndex = tokenIndex;
        var name = tokens[tokenIndex].Value;
        if (tokens[tokenIndex++].Type != TokenType.Identificador)
            return ResetExp(startIndex, out line);

        if (tokens[tokenIndex++].Type != TokenType.Assign)
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

    private bool GetCallable(Token[] tokens, out IInstruction? line)
    {
        var startIndex = tokenIndex;
        var name = tokens[tokenIndex].Value;

        if (tokens[tokenIndex++].Type != TokenType.Identificador)
            return ResetExp(startIndex, out line);

        if (tokens[tokenIndex++].Type != TokenType.ParentesisIzquierdo)
            return ResetExp(startIndex, out line);

        if (!GetParams(tokens, out IExpression[]? @params))
            return ResetExp(startIndex, out line);

        var newNode = new CallableAction(name, @params!);
        return SetExp(newNode, out line);

    }

    private bool GetParams(Token[] tokens, out IExpression[]? @params)
    {
        var startIndex = tokenIndex;
        List<IExpression> parameters = new List<IExpression> { };
        if (tokens[tokenIndex].Type != TokenType.ParentesisDerecho && GetExpression(tokens, out IExpression? value))
            parameters.Add(value!);
        while (tokens[tokenIndex].Type != TokenType.ParentesisDerecho)
        {
            if (tokens[tokenIndex++].Type != TokenType.Coma || !GetExpression(tokens, out value))
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
        => GetOrExpression(tokens, out cond);
    private bool GetOrExpression(Token[] tokens, out IExpression? cond)
        => ParseExpression(tokens, out cond, GetAndExpression, [TokenType.Or]);
    private bool GetAndExpression(Token[] tokens, out IExpression? cond)
        => ParseExpression(tokens, out cond, GetBoolLiteral, [TokenType.And]);
    private bool GetBoolLiteral(Token[] tokens, out IExpression? cond)
        => GetLiteralExp(tokens, out cond, GetBoolExpression, TokenType.Boolean);

    // TODO: Implemetar los comparadores, no son ni shift ni reduce porque no se pueden concatenar (sugerencia usar enum en vez de bool)

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
            if (tokens[tokenIndex].Type == item)
            {
                tokenIndex++;
                type = item;
                return true;
            }
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
        if (MatchToken(tokens, TokenType.Identificador))
            return SetExp(new Variable(token.Value), out exp);
        // TODO: Metodos que devuelven algo
        throw new InvalidExpressionException();
    }

    // TODO: Cambiar todos los comparadores de type por este metodo
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
