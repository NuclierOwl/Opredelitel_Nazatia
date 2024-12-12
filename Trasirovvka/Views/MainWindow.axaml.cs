using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using Brushes = Avalonia.Media.Brushes;
using Point = Avalonia.Point;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

public partial class MainWindow : Window
{
    bool Opredelenie;
    TextBlock Itog;
    string Vibronoe;
    Shape? Forma;
    Grid MainGrid;
    MainWindow()
    {
        InitializeComponent();
        Itog = new TextBlock
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Bottom,
            Margin = new Thickness(5, 5, 15, 10),
            FontSize = 35
        };
        MainGrid.Children.Add(Itog);
    }

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
            MainGrid.Children.Add(Forma);
        }
    }
    void MestoFormi(Point Tuyk)
    {
        double visota = Forma is Polygon ? 50 : Forma.Bounds.Width;
        double shirina = Forma is Polygon ? 50 : Forma.Bounds.Height;
        Canvas.SetRight(Forma, Tuyk.X - visota / 2);
        Canvas.SetTop(Forma, Tuyk.Y - shirina / 2);

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
    void NazatieOpredelitela(object? nalichie, RoutedEventArgs a)
    {
        Opredelenie = true;
    }
    IList<Point> GetMnogougolPoints(Polygon pol)
    {
        return pol.Points
            .Select(l => new Point(l.X + Canvas.GetLeft(pol), l.Y + Canvas.GetTop(pol)))
            .ToList();
    }
    bool GetKvadroTocki(Rectangle rectangle, Point poi)
    {
        double verh = Canvas.GetTop(rectangle);
        double levo = Canvas.GetLeft(rectangle);
        return poi.X >= levo && poi.X <= levo + rectangle.Width && poi.Y >= verh && poi.Y <= levo + rectangle.Height;
    }
    bool Vnutri(Shape Figura, Point Tocka)
    {
        return Figura switch
        {
            Rectangle r => GetKvadroTocki(r, Tocka),
            Polygon pol => VnutriFigur(Tocka, GetMnogougolPoints(pol)),
            _ => false
        };
    }



    bool VnutriFigur(Point poi, IList<Point> figura)
    {
        int h = figura.Count - 1;
        bool vnutri = false;
        for (int i = 0; i < figura.Count; i++)
        {
            if (figura[h].Y < poi.Y && figura[i].Y >= poi.Y || figura[i].Y < poi.Y && figura[h].Y >= poi.Y)
            {
                if (figura[i].X + (poi.Y - figura[i].Y) /
                    (figura[h].Y - figura[i].Y) * (figura[h].X - figura[i].X) < poi.X)
                {
                    vnutri = true;
                }
            }
            h = i;
        }
        return vnutri;
    }

    void Popadanie(Point Tuyk)
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
