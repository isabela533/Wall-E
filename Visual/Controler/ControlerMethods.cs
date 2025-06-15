using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using Avalonia.Media;
using AvaloniaEdit.Rendering;
using Compiler.Language;
using Compiler.Model;
using Compiler.Tokenizador;
using Tmds.DBus.Protocol;
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
            _paint.PaintWalle(x + dirX * count, y + dirY * count);
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
                size = int.Min(count, _paint.BrushSize - 1);
                DrawStep(newX, 0, size);
                DrawStep(0, newY, size);
            }
        }
    }

    public void Fill()
    {
        var (startX, startY) = _paint.WallePoss;
        var targetColor = _paint.GetColorAt(startX, startY);
        var mask = new bool[_paint.GetCanvasSize(), _paint.GetCanvasSize()];
        var fillColor = _paint.Brush;
        if (targetColor == fillColor) return;

        Queue<(int, int)> queue = new();
        queue.Enqueue((startX, startY));

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            _paint.PaintCell(x, y);
            mask[x, y] = true;

            foreach (var (dx, dy) in new (int, int)[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
            {
                int newX = x + dx, newY = y + dy;
                if (!mask[newX, newY] && _paint.IsValidPosition(newX, newY) && _paint.GetColorAt(newX, newY) == targetColor)
                    queue.Enqueue((newX, newY));
            }
        }
    }

    public void Spawn(int x, int y)
    {
        if (_paint.WalleExist)
            throw new InvalidOperationException("");
        if (!_paint.IsValidPosition(x, y))
            throw new ArgumentException("Wall-E is positioned outside the canvas");
        _paint.PaintWalle(x, y);
    }

    void Color(string color)
    {
        var colorValue = _paint.GetColorBrush(color);
        if (colorValue is null)
            throw new InvalidCastException("The color is not valid");
    }

    public void Size(int size)
    {
        if (int.IsNegative(size))
            throw new InvalidOperationException("Negative number is not available");
        _paint.GetNewSizeBrush(size);
    }

    public void DrawCircle(int dirX, int dirY, int radius)
    {
        if (radius <= 0) return;

        int x = 0;
        int y = radius;
        int x0 = _paint.WallePoss.x + dirX * radius;
        int y0 = _paint.WallePoss.y + dirY * radius;
        int d = 1 - radius;

        // Dibujar puntos iniciales (4 puntos cardinales)
        _paint.PaintCell(x0 + x, y0 + y, _paint.BrushSize);
        _paint.PaintCell(x0 + x, y0 - y, _paint.BrushSize);
        _paint.PaintCell(x0 + y, y0 + x, _paint.BrushSize);
        _paint.PaintCell(x0 - y, y0 + x, _paint.BrushSize);

        while (y >= x)
        {
            x++;

            if (d > 0)
            {
                y--;
                d += 2 * (x - y) + 1;
            }
            else
            {
                d += 2 * x + 1;
            }

            // Dibujar los 8 puntos de simetría
            _paint.PaintCell(x0 + x, y0 + y, _paint.BrushSize);
            _paint.PaintCell(x0 - x, y0 + y, _paint.BrushSize);
            _paint.PaintCell(x0 + x, y0 - y, _paint.BrushSize);
            _paint.PaintCell(x0 - x, y0 - y, _paint.BrushSize);
            _paint.PaintCell(x0 + y, y0 + x, _paint.BrushSize);
            _paint.PaintCell(x0 - y, y0 + x, _paint.BrushSize);
            _paint.PaintCell(x0 + y, y0 - x, _paint.BrushSize);
            _paint.PaintCell(x0 - y, y0 - x, _paint.BrushSize);
        }

        _paint.PaintWalle(x0, y0);
    }


    public void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
    {
        int newX = _paint.WallePoss.x + dirX * distance;
        int newY = _paint.WallePoss.y + dirY * distance;

        _paint.PaintWalle(newX - width + 1, newY - height + 1);
        DrawLine(0, 1, height * 2 - 1);
        DrawLine(1, 0, width * 2 - 1);
        DrawLine(0, -1, height * 2 - 1);
        DrawLine(-1, 0, width * 2 - 1);
        _paint.PaintWalle(newX, newY);
    }
    #endregion

    #region Functions 

    public int GetactualX() => _paint.WallePoss.x;

    public int GetActualY() => _paint.WallePoss.y;

    public int GetCanvasSize() => _paint.GetCanvasSize();

    public int GetColorCount(string color, int x1, int y1, int x2, int y2)
    {
        if (x1 < 0 || y1 < 0 || x2 >= GetCanvasSize() || y2 >= GetCanvasSize())
            throw new InvalidOperationException(); // Si alguna coordenada está fuera de los límites

        int count = 0;
        for (int i = x1; i <= x2; i++)
        {
            for (int j = y1; j <= y2; j++)
            {
                if (_paint.GetColorAt(i, j) == _paint.GetColorBrush(color))
                    count++;
            }
        }

        return count;
    }

    public bool IsBrushColor(string color)
    {
        return _paint.GetColorBrush(color) is null;
    }
    public bool IsBrushSize(int size)
    {
        return size == _paint.BrushSize;
    }
    public bool IsCanvasColor(string color, int vertical, int horizontal)
    {
        int newX = _paint.WallePoss.x + horizontal;
        int newY = _paint.WallePoss.y + vertical;
        return _paint.GetColorBrush(color) == _paint.GetColorAt(newX, newY);
    }

    #endregion



    #region Tools
    private void DrawStep(int dirX, int dirY, int distance)
    {
        (int x, int y) = _paint.WallePoss;
        for (int i = 1; i <= distance; i++)
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
                return new ValueType(TokenType.Num, GetCanvasSize());
            case "getcolorcount":
                return new ValueType(TokenType.Num, GetColorCount((string)paramValues[0].Value, (int)paramValues[1].Value, (int)paramValues[2].Value, (int)paramValues[3].Value, (int)paramValues[4].Value));
            case "isbrushcolor":
                return new ValueType(TokenType.Boolean, IsBrushColor((string)paramValues[0].Value));
            case "isbrushsize":
                return new ValueType(TokenType.Boolean, IsBrushSize((int)paramValues[0].Value));
            case "iscanvascolor":
                return new ValueType(TokenType.Boolean, IsCanvasColor((string)paramValues[0].Value, (int)paramValues[1].Value, (int)paramValues[2].Value));
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
                Fill();
                break;

            case "spawn":
                int x = (int)paramValues[0].Value;
                int y = (int)paramValues[1].Value;
                Spawn(x, y);
                break;

            case "color":
                string color = (string)paramValues[0].Value;
                Color(color);
                break;

            case "size":
                int size = (int)paramValues[0].Value;
                Size(size);
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
                DrawCircle(dirX, dirY, radius);
                break;
            case "drawrectangle":
                dirX = (int)paramValues[0].Value;
                dirY = (int)paramValues[1].Value;
                distance = (int)paramValues[2].Value;
                int width = (int)paramValues[3].Value;
                int height = (int)paramValues[4].Value;
                DrawRectangle(dirX, dirY, distance, width, height);
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
