using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia;
using System;
using System.Text;

using Visual.Interface;
using Visual.Controler;
using Compiler.Model;
using Compiler.Parser;
using Compiler.Tokenizador;
using Avalonia.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using Compiler.Language;
namespace Visual;
public partial class MainWindow : Window, IPaint
{
    #region prop
    public Border[,] Map;
    private ControlerMethods? controler;
    private Image WalleImage;
    private (int x, int y) _wallePoss;
    public IBrush Brush { get; set; }
    public int BrushSize { get; set; }
    public bool WalleExist { get; set; }
    public (int x, int y) WallePoss
    {
        get { return _wallePoss; }
        set { _wallePoss = value; }
    }
    #endregion
    public MainWindow()
    {
        InitializeComponent();
        controler = new ControlerMethods(this);
        int filas = PixelGrid.RowDefinitions.Count;
        int columnas = PixelGrid.ColumnDefinitions.Count;
        WalleImage = new Image() { Source = new Bitmap(@"D:\Wall-E\Visual\Assents\Píxel Art de Wally.jpg") };
        WalleExist = false;
        Map = new Border[filas, columnas];
        Brush = Brushes.Transparent;
        BrushSize = 1;
        _wallePoss = (0, 0);
    }


    #region tools 
    private void GetErrors(List<string> errores)
    {
        // Limpiar errores previos
        ErrorsPanel.Text = string.Empty;

        if (errores != null && errores.Count > 0)
        {
            var sbErrores = new StringBuilder();
            sbErrores.AppendLine("Methods errors detected:");
            foreach (var error in errores)
            {
                sbErrores.AppendLine($"- {error}"); // Ajustar formato según estructura del error (línea, col, mensaje)
            }
            ErrorsPanel.Text = sbErrores.ToString();
            return;
        }
    }

    public void PaintWalle(int x, int y)
    {
        WalleExist = true;
        Map[WallePoss.x, WallePoss.y].Child = null;
        Map[x, y].Child = WalleImage;
        WallePoss = (x, y);
    }

    public IBrush GetColorAt(int x, int y)
    {
        if (x < 0 || x >= Map.GetLength(1) || y < 0 || y >= Map.GetLength(0))
            return null!;
        return Map[x, y].Background!;
    }

    public bool IsValidPosition(int x, int y)
    {
        return !(x < 0 || x >= Map.GetLength(0) || y < 0 || y >= Map.GetLength(1));
    }

    public void PaintCell(int x, int y, int size)
    {
        if (!IsValidPosition(x, y)) throw new IndexOutOfRangeException();
        if (Brush == Brushes.Transparent)
            return;
        var cor = (size - 1) / 2;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int newX = x - cor + i;
                int newY = y - cor + j;
                Map[newX, newY].Background = Brush;
            }
        }
    }

    public IBrush? GetColorBrush(string color)
    {
        return color[1..^1].ToLowerInvariant() switch
        {
            "red" => Brushes.Red,
            "blue" => Brushes.Blue,
            "green" => Brushes.Green,
            "yellow" => Brushes.Yellow,
            "orange" => Brushes.Orange,
            "purple" => Brushes.Purple,
            "black" => Brushes.Black,
            "white" => Brushes.White,
            "transparent" => Brushes.Transparent,
            _ => null
        };
    }

    public IBrush? SetColorBrush(string color)
    {
        var colorBrush = GetColorBrush(color);
        if (colorBrush == null)
            throw new ArgumentException("Color is not valid");
        Brush = colorBrush;
        return Brush;
    }

    public int GetNewSizeBrush(int k)
    {
        BrushSize = k - (k + 1) % 2;
        return BrushSize;
    }

    public int GetCanvasSize() => PixelGrid.RowDefinitions.Count;

    #endregion

    private void CrearGridDefinitions(decimal? count)
    {
        ClearCanvas();

        PixelGrid.RowDefinitions.Clear();
        PixelGrid.ColumnDefinitions.Clear();

        var length = new GridLength(1, GridUnitType.Star);

        for (int i = 0; i < count; i++)
        {
            PixelGrid.RowDefinitions.Add(new RowDefinition(length));
            PixelGrid.ColumnDefinitions.Add(new ColumnDefinition(length));
        }

        int filas = PixelGrid.RowDefinitions.Count;
        int columnas = PixelGrid.ColumnDefinitions.Count;
        Map = new Border[filas, columnas];
        CrearCuadrosConBorde();
    }

    [Obsolete]
    private async void OnExportarClick(object? sender, RoutedEventArgs e)
    {
        var dialogo = new SaveFileDialog
        {
            Title = "Guardar archivo .pw",
            DefaultExtension = "pw",
            Filters =
            {
                new FileDialogFilter
                {
                    Name = "Archivo .pw",
                    Extensions = { "pw" }
                }
            }
        };

        var archivo = await dialogo.ShowAsync(this);

        if (!string.IsNullOrWhiteSpace(archivo))
        {
            string saveContent = Editor.Text; //archivo que quiero exportar que es mi text del editor
            await File.WriteAllTextAsync(archivo, saveContent);
        }
    }

    [Obsolete]
    private async void OnImportarClick(object? sender, RoutedEventArgs e)
    {
        var dialogo = new OpenFileDialog
        {
            Title = "Importar archivo .pw",
            AllowMultiple = false,
            Filters =
            {
                new FileDialogFilter
                {
                    Name = "Archivo .pw",
                    Extensions = { "pw" }
                }
            }
        };

        var archivo = await dialogo.ShowAsync(this);

        if (archivo?.Length > 0 && File.Exists(archivo[0]))
        {
            var contenido = await File.ReadAllTextAsync(archivo[0]);
            Editor.Text = contenido; // poner el contenido leido del archivo en el text editor
        }
    }

    private void CrearCuadrosConBorde()
    {
        int filas = PixelGrid.RowDefinitions.Count;
        int columnas = PixelGrid.ColumnDefinitions.Count;
        PixelGrid.Children.Clear();
        for (int fila = 0; fila < filas; fila++)
        {
            for (int columna = 0; columna < columnas; columna++)
            {
                var cuadro = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.White,
                };
                // Posicionar dentro del Grid
                Grid.SetRow(cuadro, fila);
                Grid.SetColumn(cuadro, columna);
                // Añadir al Grid
                PixelGrid.Children.Add(cuadro);
                Map[fila, columna] = cuadro;
            }
        }
    }

    private void TextChanged(object sender, EventArgs e)
    {
        var tokenizador = new Tokenizador();
        var parser = new Parser();
        string code = Editor.Text;
        // Limpiar errores previos
        ErrorsPanel.Text = string.Empty;

        Token[] tokens = tokenizador.Tokenizar(code);
        if (tokenizador.errores != null && tokenizador.errores.Count > 0)
        {
            var sbErrores = new StringBuilder();
            sbErrores.AppendLine("Tokenization errors detected:");
            foreach (var error in tokenizador.errores)
            {
                sbErrores.AppendLine($"- {error}"); // Ajustar formato según estructura del error (línea, col, mensaje)
            }
            ErrorsPanel.Text = sbErrores.ToString();
            return;
        }

        try
        {
            var ast = parser.Parse(tokens);
            ErrorsPanel.Text = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorsPanel.Text = $"Parsing error: {ex.Message}";
        }
    }

    private void RunCode_Click(object sender, RoutedEventArgs e)
    {
        ClearCanvas();
        var parser = new Parser();
        var context = new Context(controler);

        string code = Editor.Text;
        var tokenizador = new Tokenizador();
        Token[] tokens = tokenizador.Tokenizar(code);
        try
        {
            // TODO: cambiar a ingles
            var ast = parser.Parse(tokens) as BlockExpression;
            if (ast?.Lines.Count == 0 
                || ast?.Lines[0] is not CallableAction action 
                || !action.Name.Equals("spawn", StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidProgramException("Falta el spawn");
            ast.Accept(context);
        }
        catch (Exception ex)
        {
            ErrorsPanel.Text = $"Error: {ex.Message}";
        }
    }

    private void ClearCanvas()
    {
        if (IsValidPosition(WallePoss.x, WallePoss.y) && Map[WallePoss.x, WallePoss.y] is not null)
            Map[WallePoss.x, WallePoss.y].Child = null;
        WalleExist = false;

        foreach (var item in PixelGrid.Children)
        {
            if (item is Border border)
                border.Background = Brushes.White;
        }
    }

    private void NumericValueChanged(object sender, RoutedEventArgs e)
        => CrearGridDefinitions((sender as NumericUpDown)!.Value);

    private void ResetCanvasClick(object sender, RoutedEventArgs e)
        => ClearCanvas();

    private void SplitView_PaneClosed(object sender, RoutedEventArgs e)
        => PixelGrid.Margin = new Thickness(0, 0, 0, 0);

    private void SplitView_PaneOpened(object sender, RoutedEventArgs e)
        => PixelGrid.Margin = new Thickness((sender as SplitView)!.OpenPaneLength, 0, 0, 0);

    private void SplitEditor_Click(object sender, RoutedEventArgs e)
        => SplitPanel.IsPaneOpen = !SplitPanel.IsPaneOpen;
}
