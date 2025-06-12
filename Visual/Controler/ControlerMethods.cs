using System;
using System.Runtime.InteropServices;
using AvaloniaEdit.Rendering;
using Compiler.Language;
using Compiler.Model;
using Compiler.Tokenizador;
using Visual.Interface;
using ValueType = Compiler.Language.ValueType;

namespace Visual.Controler;

public class ControlerMethods(IPaint paint) : IContextCallable
{
    private readonly IPaint _paint = paint;
#region Instructions 
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
#endregion

#region Functions 
#endregion



#region Tools
    private void DrawStep(int dirX, int dirY, int distance)
    {
        (int x, int y) = _paint.WallePoss;
        for (int i = 1; i < distance; i++)
            _paint.PaintCell(x + dirX * i, y + dirY * i);
    }

    // TODO: REVISAR Implementar para poder usarlo como IContextCallable en el contexto del Compiler
    // TODO: REVISAR Considerar hacer un metodo auxiliar para verificar si los parametros estan correctos (cantidad y tipos) 
    public ValueType CallFunction(string name, ValueType[] paramValues)
    {
        if (!CheckParamsType(name, paramValues) || !CheckParamsCount(name, paramValues, out int countParams))
            throw new ArgumentException("The parameters do not match with the function");

        switch (name.ToLowerInvariant())
        {

            case "getactualx":
                return new ValueType(TokenType.Num, _paint.WallePoss.x);
            case "getactualy":
                return new ValueType(TokenType.Num, _paint.WallePoss.y);
            case "getcanvassize":
                return new ValueType(TokenType.Num, _paint.GetCanvasSize());
            case "getcolorcount":
                return new ValueType(TokenType.Num, _paint.GetColorCount((string)paramValues[0].Value, (int)paramValues[1].Value, (int)paramValues[2].Value, (int)paramValues[3].Value, (int)paramValues[4].Value));
            case "isbrushcolor":
                return new ValueType(TokenType.Boolean, _paint.IsBrushColor((string)paramValues[0].Value));
            case "isbrushsize":
                return new ValueType(TokenType.Boolean, _paint.IsBrushSize((int)paramValues[0].Value));
            case "iscanvascolor":
                return new ValueType(TokenType.Boolean, _paint.IsCanvasColor((string)paramValues[0].Value, (int)paramValues[1].Value, (int)paramValues[2].Value));
            default:
                throw new ArgumentException("No existe la funcion " + name);

        }
    }

    public void ExecuteAction(string name, ValueType[] paramValues)
    {
        if (!CheckParamsType(name, paramValues) || !CheckParamsCount(name, paramValues, out _))
            throw new ArgumentException("The parameters do not match with the function");

        switch (name.ToLowerInvariant())
        {
            case "fill":
                _paint.Fill();
                break;

            case "spawn":
                int x = (int)paramValues[0].Value;
                int y = (int)paramValues[1].Value;
                _paint.Spawn(x, y);
                break;

            case "color":
                string color = (string)paramValues[0].Value;
                _paint.Color(color);
                break;

            case "size":
                int size = (int)paramValues[0].Value;
                _paint.Size(size);
                break;

            case "drawline":
                int dirX = (int)paramValues[0].Value;
                int dirY = (int)paramValues[1].Value;
                int distance = (int)paramValues[2].Value;
                DrawLine(dirX, dirY, distance);
                break;
            case "drawcircle":
                dirX = (int)paramValues[0].Value;
                dirY = (int)paramValues[1].Value;
                int radius = (int)paramValues[2].Value;
                _paint.DrawCircle(dirX, dirY, radius);
                break;
            case "drawrectangle":
                dirX = (int)paramValues[0].Value;
                dirY = (int)paramValues[1].Value;
                distance = (int) paramValues[2].Value;
                int width = (int)paramValues[3].Value;
                int height = (int)paramValues[4].Value;
                _paint.DrawRectangle(dirX, dirY, distance, width, height);
                break;
            default:
                throw new ArgumentException("No existe la accion " + name);
        
        }
    }

    public bool CheckParamsType(string name, ValueType[] paramValues)
    {
        if (name == null) return false;
        int cantParameters;
        CheckParamsCount(name, paramValues, out cantParameters);

        if (cantParameters == 0) return true; //si no hay parametros, retorna true porque no hay tipos para comparar
        if (paramValues.Length != cantParameters) return false; // si mla cantidad de parametros no coinicide, retorna falso 

        switch (name.ToLowerInvariant())
        {
            case "getactualx":
            case "getactualy":
            case "getcanvassize":
            case "fill":
                return paramValues.Length == 0;

            case "spawn":
                return paramValues[0].Kind == TokenType.Num &&
                    paramValues[1].Kind == TokenType.Num;

            case "color":
                return paramValues[0].Kind == TokenType.String;

            case "size":
                return paramValues[0].Kind == TokenType.Num;

            case "drawline":
                return paramValues[0].Kind == TokenType.Num &&
                     paramValues[1].Kind == TokenType.Num &&
                     paramValues[2].Kind == TokenType.Num;

            case "drawcircle":
                return paramValues[0].Kind == TokenType.Num &&
                    paramValues[1].Kind == TokenType.Num &&
                    paramValues[2].Kind == TokenType.Num;

            case "drawrectangle":
                return paramValues[0].Kind == TokenType.Num &&
                    paramValues[1].Kind == TokenType.Num &&
                    paramValues[2].Kind == TokenType.Num &&
                    paramValues[3].Kind == TokenType.Num &&
                    paramValues[4].Kind == TokenType.Num;

            case "getcolorcount":
                return paramValues[0].Kind == TokenType.String &&
                   paramValues[1].Kind == TokenType.Num &&
                   paramValues[2].Kind == TokenType.Num &&
                   paramValues[3].Kind == TokenType.Num &&
                   paramValues[4].Kind == TokenType.Num;

            case "isbrushcolor":
                return paramValues[0].Kind == TokenType.String;

            case "isbrushsize":
                return paramValues[0].Kind == TokenType.Num;

            case "iscanvascolor":
                return paramValues[0].Kind == TokenType.String &&
                    paramValues[1].Kind == TokenType.Num &&
                    paramValues[2].Kind == TokenType.Num;

            default:
                return false;
        }
    }

    public bool CheckParamsCount(string name, ValueType[] paramValues, out int cantParams)
    {
        cantParams = 0;
        if (name == null) return false;

        switch (name.ToLowerInvariant())
        {
            case "getactualx":
            case "getactualy":
            case "getcanvassize":
            case "fill":
                cantParams = 0;
                return paramValues.Length == 0;

            case "spawn":
                cantParams = 2;
                return paramValues.Length == 2;

            case "color":
                cantParams = 1;
                return paramValues.Length == 1;

            case "size":
                cantParams = 1;
                return paramValues.Length == 1;

            case "drawline":
                cantParams = 3;
                return paramValues.Length == 3;

            case "drawcircle":
                cantParams = 3;
                return paramValues.Length == 3;

            case "drawrectangle":
                cantParams = 5;
                return paramValues.Length == 5;

            case "getcolorcount":
                cantParams = 5;
                return paramValues.Length == 5;

            case "isbrushcolor":
                cantParams = 1;
                return paramValues.Length == 1;

            case "isbrushsize":
                cantParams = 1;
                return paramValues.Length == 1;

            case "iscanvascolor":
                cantParams = 3;
                return paramValues.Length == 3;

            default:
                return false;
        }
    }
#endregion
}
