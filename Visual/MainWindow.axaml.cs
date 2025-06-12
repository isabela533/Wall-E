using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using Avalonia;
using System;

using Visual.Interface;
using Visual.Controler;
using Compiler.Model;
using Compiler.Parser;
using Compiler.Tokenizador;
namespace Visual;

public partial class MainWindow : Window, IPaint
{
    private Border[,] Map;
    private ControlerMethods controler;
    //TODO: hacer esto
    public Brush Brush { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int BrushSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public (int x, int y) WallePoss { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public MainWindow()
    {
        InitializeComponent();
        controler = new ControlerMethods(this);
        int filas = PixelGrid.RowDefinitions.Count;
        int columnas = PixelGrid.ColumnDefinitions.Count;
        Map = new Border[filas, columnas];
    }

    private void CrearGridDefinitions(decimal? count)
    {
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

    //TODO: Hacer que muestre los errores antes de ejecutar (cada vez que escribe)
    private void TextChanged(object sender, EventArgs e)
    {

    }
    
    private void RunCode_Click(object sender, RoutedEventArgs e)
    {
        var parser = new Parser();
        var context = new Context(controler);;

        string code = Editor.Text;
        Token[] tokens = Tokenizador.Tokenizar(code);
        var ast = parser.Parse(tokens);
        ast.Accept(context);
    }

    private void NumericValueChanged(object sender, RoutedEventArgs e)
        => CrearGridDefinitions((sender as NumericUpDown)!.Value);

    private void ClearCanvasClick(object sender, RoutedEventArgs e)
        => CrearCuadrosConBorde();

    private void SplitView_PaneClosed(object sender, RoutedEventArgs e)
        => PixelGrid.Margin = new Thickness(0, 0, 0, 0);

    private void SplitView_PaneOpened(object sender, RoutedEventArgs e)
        => PixelGrid.Margin = new Thickness((sender as SplitView)!.OpenPaneLength, 0, 0, 0);

    private void SplitEditor_Click(object sender, RoutedEventArgs e)
        => SplitPanel.IsPaneOpen = !SplitPanel.IsPaneOpen;

    public void PaintCell(int x, int y)
        => Map[y, x].Background = Brush;

    public void PaintWalle(int x, int y)
    {
        throw new NotImplementedException();
    }
}
