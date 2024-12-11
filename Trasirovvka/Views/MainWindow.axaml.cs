using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Drawing;
using Brushes = Avalonia.Media.Brushes;
using Point = Avalonia.Point;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

public partial class MainWindow : Window
{
    bool Opredelenie;
    TextBox Itog;
    string Vibronoe;
    Shape? Forma;

    public MainWindow()
    {
        InitializeComponent();
        Itog = new TextBlock
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
            Margin = new Thickness(5, 5, 15, 10),
            FontSize = 35
        };
        Glavnoe.Posledstvie.Add(Itog);
    }

    /*
      public void Nazatie(string f)
         {
             Vibronoe = f;
             Modik = false;
         }
    */
    void UpdateItog(string Rezultat, IBrush cvet)
    {
        Itog.Text = Rezultat;
        Itog.Foreground = cvet;
        Console.WriteLine(Rezultat);
    }
    Rectangle CreateRectangle() => new Rectangle
    {
        Width = 60,
        Height = 60,
        Fill = Brushes.DeepPink
    };
    Polygon CreatePolygon(IEnumerable<Point> points, IBrush fill) => new Polygon
    {
        Points = new AvaloniaList<Point>(points),
        Fill = fill
    };
    void Otrisovka(Point Tuyk)
    {
        Forma = Vibronoe switch
        {
            "Квдрат" => CreateRectangle(),
            "Прямоугольник" => CreatePolygon(new List<Point>
            {
                new Point (30, 0),
                new Point (70, 25),
                new Point (40, 50),
                new Point (20, 60),
                new Point (0, 25)
            }, Brushes.Aqua),
            "Ромб" => CreatePolygon(new List<Point>
            {
                new Point(25, 0),
                new Point(50, 13),
                new Point(50, 38),
                new Point(25, 50),
                new Point(0, 38),
                new Point(0, 13)
            }, Brushes.PaleVioletRed),
            _ => null
        };
        if (Forma != null)
        {
            MestoFormi(Tuyk);
            Glavnoe.Posledstvie.Add(Forma);
        }
    }
    void MestoFormi(Point Tuyk)
    {
        double visota = Forma is Polygon ? 50 : Forma.Bounds.Width;
        double shirina = Forma is Polygon ? 50 : Forma.Bounds.Height;
        Canvas.SetRight(Forma, Tuyk.X - visota /2);
        Canvas.SetTop(Forma, Tuyk.Y - shirina /2);

    }
    private void TuykKvadro(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Vibronoe = "Это квадрат";
        Opredelenie = false;
    }
    private void TuykPramo(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Vibronoe = "Это триугольник";
        Opredelenie = false;
    }
    private void TuykRomb(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Vibronoe = "Это ромб";
        Opredelenie = false;
    }
    public void NazatieOpredelitela(object? nalichie, RoutedEventArgs a)
    {
        Opredelenie = true;
    }

    public bool Vnutri(Shape Figura, Point Tocka)
    {
        if (Figura is Rectangle pramoug)
        {

        }
    }



    public void Popadanie(Point Tuyk)
    {
        if (Opredelenie)
        {
            if (Forma != null && Vnutri(Forma, Tuyk))
            {
                UpdateItog("Попадание!", Brushes.Lime);
            }
            else
            {
                UpdateItog("Не попал!", Brushes.Crimson);
            }
        }
    }

}
