using Avalonia.Media;

namespace Visual.Interface;

public interface IPaint
{
    Brush Brush { get; set; }
    int BrushSize { get; set; }
    (int x, int y) WallePoss { get; set; }
    int GetCanvasSize();
    int GetColorCount(string color, int x1, int y1, int x2, int y2);
    bool IsBrushColor(string color);
    bool IsBrushSize(int size);
    bool IsCanvasSize(string color, int vertical, int horizontal);

    // TODO: Cambiar el valor de retorno para que admita un bool (que diga si se pudo realizar esta accion)
    void PaintCell(int x, int y);
    void  PaintWalle(int x, int y);
}
