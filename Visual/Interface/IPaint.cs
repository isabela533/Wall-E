using Avalonia.Media;

namespace Visual.Interface;

public interface IPaint
{
    Brush Brush { get; set; }
    int BrushSize { get; set; }
    (int x, int y) WallePoss { get; set; }
    #region Instructions
    void Fill();
    void Spawn(int x, int y);
    void Color(string color);
    void Size(int size);
    void DrawCircle(int dirX, int dirY, int radius);
    void DrawRectangle(int dirX, int dirY, int distance, int width, int height);
    #endregion 
    
    #region Functions
    int GetCanvasSize();
    int GetColorCount(string color, int x1, int y1, int x2, int y2);
    bool IsBrushColor(string color);
    bool IsBrushSize(int size);
    bool IsCanvasColor(string color, int vertical, int horizontal);
    #endregion

    // TODO: Cambiar el valor de retorno para que admita un bool (que diga si se pudo realizar esta accion)
    void PaintCell(int x, int y);
    void  PaintWalle(int x, int y);
}
