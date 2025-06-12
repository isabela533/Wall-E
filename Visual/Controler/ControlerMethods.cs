using System;
using System.Runtime.InteropServices;
using Compiler.Language;
using Compiler.Model;
using Compiler.Tokenizador;
using Visual.Interface;
using ValueType = Compiler.Language.ValueType;

namespace Visual.Controler;

public class ControlerMethods(IPaint paint) : IContextCallable
{
    private readonly IPaint _paint = paint;

    public void DrawLine(int dirX, int dirY, int distance)
    {
        (int x, int y) = _paint.WallePoss;
        for (int count = 0; count < distance; count++)
        {
            // Mover Walle y actualizar posicion
            _paint.WallePoss = (x + dirX * count, y + dirY * count);
            _paint.PaintCell(_paint.WallePoss.x, _paint.WallePoss.y);

            int size;
            int newX;
            int newY;
            if (dirX * dirY == 0)
            {
                newX = (dirX + 1) % 2; // direccion perpendicular
                newY = (dirY + 1) % 2; // direccion perpendicular

                // Pintar el resto de la brocha
                size = _paint.BrushSize / 2;
                DrawStep(newX, newY, size);
                DrawStep(-newX, -newY, size);
            }
            else
            {
                newX = -dirX; // direccion contraria
                newY = -dirY; // direccion contraria

                // Pintar el resto de la brocha
                size = int.Min(count, _paint.BrushSize);
                DrawStep(newX, 0, size);
                DrawStep(0, newY, size);
            }
        }
    }


    #region Tools

    private void DrawStep(int dirX, int dirY, int distance)
    {
        (int x, int y) = _paint.WallePoss;
        for (int i = 1; i < distance; i++)
            _paint.PaintCell(x + dirX * i, y + dirY * i);
    }

    // TODO: Implementar para poder usarlo como IContextCallable en el contexto del Compiler
    // TODO: Considerar hacer un metodo auxiliar para verificar si los parametros estan correctos (cantidad y tipos)
    public ValueType CallFunction(string name, ValueType[] paramValues)
    {
        if (!CheckParams(name, paramValues))
            throw new ArgumentException();

        switch (name.ToLowerInvariant())
        {

            case "getactualx":
                return new ValueType(TokenType.Num, _paint.WallePoss.x);
            case "getactualy":
                return new ValueType(TokenType.Num, _paint.WallePoss.y);
            case "getcanvassize":
                return new ValueType(TokenType.Num, _paint.GetCanvasSize());
            case "getcolorcount":
                return new ValueType(TokenType.Num, _paint.GetColorCount((string)paramValues[0].Value, (int) paramValues[1].Value, (int) paramValues[2].Value, (int) paramValues[3].Value, (int) paramValues[4].Value));
            case "isbrushcolor":
                return new ValueType(TokenType.Boolean, _paint.IsBrushColor((string)paramValues[0].Value));
            case "isbrushsize":
                return new ValueType(TokenType.Boolean, _paint.IsBrushSize((int)paramValues[0].Value));
            case "iscanvassize":
                return new ValueType(TokenType.Boolean, _paint.IsCanvasSize((string)paramValues[0].Value, (int)paramValues[1].Value, (int)paramValues[2].Value));
            default: 
            throw new ArgumentException("No existe la funcion " + name);

        }
    }

    public void ExecuteAction(string name, ValueType[] paramValues)
    {
        throw new System.NotImplementedException();
    }

    public bool CheckParams(string name, ValueType[] paramValues)
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
