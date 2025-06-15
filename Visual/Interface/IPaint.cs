using Avalonia.Media;

namespace Visual.Interface;

public interface IPaint
{
    IBrush Brush { get; set; }
    int BrushSize { get; set; }
    (int x, int y) WallePoss { get; set; }
    bool WalleExist { get; set; }
    void PaintCell(int x, int y, int size = 1);
    void PaintWalle(int x, int y);
    bool IsValidPosition(int x, int y);
    IBrush GetColorAt(int x,int y);
    IBrush? GetColorBrush(string color);
    int GetNewSizeBrush(int k);
    int GetCanvasSize();
}
