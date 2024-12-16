using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using Brushes = Avalonia.Media.Brushes;
using Point = Avalonia.Point;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

namespace Trasirovvka.Views;

public partial class MainWindow : Window
{
    bool Opredelenie;
    TextBlock Itog;
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
        Format.Children.Add(Itog);
    }


    void UpdateItog(string Rezultat, IBrush cvet)
    {
        Itog.Text = Rezultat;
        Itog.Foreground = cvet;
        Console.WriteLine(Rezultat);
    }


    Rectangle CreateKvadro() => new Rectangle
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
            "Квадрат" => CreateKvadro(),
            "4-х угольник" => CreatePolygon(new List<Point>
        {
            new Point (30, 0),
            new Point (70, 25),
            new Point (40, 50),
            new Point (25, 70),
            new Point (0, 30)
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
            Pole.Children.Add(Forma);
        }
    }


    void MestoFormi(Point Tap)
    {
        double visota =Forma is Polygon ? 50 : Forma.Bounds.Height;
        double shirina = Forma is Polygon ? 50 : Forma.Bounds.Width;
        Canvas.SetLeft(Forma, Tap.X - shirina / 2);
        Canvas.SetTop(Forma, Tap.Y - visota / 2);
    }

    void TuykKvadro(object? sender, Avalonia.Interactivity.RoutedEventArgs a)
    {
        Vibronoe = "Квадрат";
    }

    void TuykPramo(object? sender, Avalonia.Interactivity.RoutedEventArgs a)
    {
        Vibronoe = "4-х угольник";
    }

    void TuykRomb(object? sender, Avalonia.Interactivity.RoutedEventArgs a)
    {
        Vibronoe = "Ромб";
    }

    void NazatieOpredelitela(object? nalichie, RoutedEventArgs a)
    {
        Opredelenie = true;
    }


    void NazatieVOkno(object? nalichie, PointerPressedEventArgs a)
    {
        var pointerPosition = a.GetPosition(Pole);

        if (Opredelenie)
        {
            Popadanie(pointerPosition);
            return;
        }

        if (string.IsNullOrEmpty(Vibronoe)) return;

        Pole.Children.Clear();
        Otrisovka(pointerPosition);
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
                    vnutri = !vnutri;
                }
            }
            h = i;
        }
        return vnutri || NaStorone(poi, figura);
    }


    bool NaStorone(Point poi, IList<Point> figura)
    {
        for (int i = 0; i < figura.Count; i++)
        {
            var nachalo = figura[i];
            var konec = figura[(i + 1) % figura.Count];
            if (NaLinie(poi, nachalo, konec))
            {
                return true;
            }
        }
        return false;
    }
    bool NaLinie(Point pervoe, Point vtoroe, Point tretie)
    {
        double crossProduct = (pervoe.Y - vtoroe.Y) * (tretie.X - vtoroe.X) - (pervoe.X - vtoroe.X) * (tretie.Y - vtoroe.Y);
        if (Math.Abs(crossProduct) > 0.0001) return false;

        double dotProduct = (pervoe.X - vtoroe.X) * (tretie.X - vtoroe.X) + (pervoe.Y - vtoroe.Y) * (tretie.Y - vtoroe.Y);
        if (dotProduct < 0) return false;

        double squaredLengthBA = (tretie.X - vtoroe.X) * (tretie.X - vtoroe.X) + (tretie.Y - vtoroe.Y) * (tretie.Y - vtoroe.Y);
        return dotProduct <= squaredLengthBA;
    }
    async void Popadanie(Point Tuyk)
    {
        if (Opredelenie)
        {
            if (Forma != null)
            {
                if (Vnutri(Forma,Tuyk))
                {
                    UpdateItog($"Попадание! \nФигура: {Vibronoe}", Brushes.Lime);
                }
                else
                {
                    UpdateItog("Не попал!", Brushes.Crimson);
                }
            }
            else
            {
                UpdateItog("Здесь нечего нет!", Brushes.Orange);
            }
        }
    }

}