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
using System.Threading.Tasks;
using Brushes = Avalonia.Media.Brushes;
using Point = Avalonia.Point;
using Rectangle = Avalonia.Controls.Shapes.Rectangle;

namespace Trasirovvka.Views;

public partial class MainWindow : Window
{
    Random sluchai = new Random();
    bool Opredelenie;
    TextBlock Itog;
    string Vibronoe;
    Shape? Forma;
    public MainWindow()// �������� ����
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

    void UpdateItog(string Rezultat, IBrush cvet) // ���������� ���������� ������
    {
        Itog.Text = Rezultat;
        Itog.Foreground = cvet;
        Console.WriteLine(Rezultat);
    }

    Polygon CreatePolygon(IEnumerable<Point> points, IBrush fill) => new Polygon // ������� ������
    {
        Points = new AvaloniaList<Point>(points),
        Fill = fill
    };


    void Otrisovka(Point Tuyk) // ����������� ����� ������ ���������� �������
    {
        Forma = Vibronoe switch
        {
            "�������" => CreatePolygon(new List<Point>
        {
            new Point (0, 0),
            new Point (60, 0),
            new Point (60, 60),
            new Point (0, 60)
        }, Brushes.DeepPink),
            "4-� ��������" => CreatePolygon(new List<Point>
        {
            new Point (30, 0),
            new Point (70, 25),
            new Point (40, 50),
            new Point (25, 70),
            new Point (0, 30)
        }, Brushes.Aqua),
            "����" => CreatePolygon(new List<Point>
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


    void MestoFormi(Point Tap) // ����������� ����� ���������� ������
    {
        double visota = Forma is Polygon ? 50 : Forma.Bounds.Height;
        double shirina = Forma is Polygon ? 50 : Forma.Bounds.Width;
        Canvas.SetLeft(Forma, Tap.X - shirina / 2);
        Canvas.SetTop(Forma, Tap.Y - visota / 2);
    }

    void TuykKvadro(object? sender, Avalonia.Interactivity.RoutedEventArgs a)// ������ ��� ��������
    {
        Vibronoe = "�������";
        Opredelenie = false;
    }

    void TuykPramo(object? sender, Avalonia.Interactivity.RoutedEventArgs a) // ������ ��� 4-� ���������
    {
        Vibronoe = "4-� ��������";
        Opredelenie = false;
    }

    void TuykRomb(object? sender, Avalonia.Interactivity.RoutedEventArgs a) // ������ ��� �����
    {
        Vibronoe = "����";
        Opredelenie = false;
    }

    void NazatieOpredelitela(object? nalichie, RoutedEventArgs a)// ������ ��� ������������ ���������
    {
        Opredelenie = true;
    }

    void NazatieRandoma(object? nalichie, RoutedEventArgs a) // ������ ��� ��������� ������� ������
    {
        Pole.Children.Clear();
        Raskidka();
    }

    void Obnulenie(object? nalichie, RoutedEventArgs a) // ������ ��� ������� ������
    {
        Pole.Children.Clear();
        Opredelenie = false;
    }


    void NazatieVOkno(object? nalichie, PointerPressedEventArgs a) // ����������� ��������� � ����
    {
        var risovka = a.GetPosition(Pole);

        if (Opredelenie)
        {
            Popadanie(risovka);
            return;
        }

        if (string.IsNullOrEmpty(Vibronoe)) return;

        Pole.Children.Clear();
        Otrisovka(risovka);
    }


    IList<Point> GetMnogougolPoints(Polygon pol) // ��������� ����� �������������
    {
        return pol.Points
            .Select(l => new Point(l.X + Canvas.GetLeft(pol), l.Y + Canvas.GetTop(pol)))
            .ToList();
    }


    bool GetFiguraTocki(Rectangle rectangle, Point poi) // ����������� ��������� � ������
    {
        double verh = Canvas.GetTop(rectangle);
        double levo = Canvas.GetLeft(rectangle);
        return poi.X >= levo && poi.X <= levo + rectangle.Width && poi.Y >= verh && poi.Y <= levo + rectangle.Height;
    }


    bool Vnutri(Shape Figura, Point Tocka) // ����������� ��������� �� ������� ������
    {
        return Figura switch
        {
            Rectangle r => GetFiguraTocki(r, Tocka),
            Polygon pol => VnutriFigur(Tocka, GetMnogougolPoints(pol)),
            _ => false
        };
    }


    bool VnutriFigur(Point poi, IList<Point> figura) // ����������� ���������� ����� ������ ������
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


    bool NaStorone(Point poi, IList<Point> figura) // ����������� ���������� ����� �� ������� ������
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
    bool NaLinie(Point pervoe, Point vtoroe, Point tretie) // ����������� ���������� ����� �� �����
    {
        double crossProduct = (pervoe.Y - vtoroe.Y) * (tretie.X - vtoroe.X) - (pervoe.X - vtoroe.X) * (tretie.Y - vtoroe.Y);
        if (Math.Abs(crossProduct) > 0.0001) return false;

        double dotProduct = (pervoe.X - vtoroe.X) * (tretie.X - vtoroe.X) + (pervoe.Y - vtoroe.Y) * (tretie.Y - vtoroe.Y);
        if (dotProduct < 0) return false;

        double squaredLengthBA = (tretie.X - vtoroe.X) * (tretie.X - vtoroe.X) + (tretie.Y - vtoroe.Y) * (tretie.Y - vtoroe.Y);
        return dotProduct <= squaredLengthBA;
    }
    void Popadanie(Point Tuyk) // ������������ ���������
    {
        if (Opredelenie)
        {
            if (Forma != null && Vnutri(Forma, Tuyk))
            {
                UpdateItog($"���������! \n������: {Vibronoe}", Brushes.Lime);
            }
            else
            {
                var figurs = new List<String> { "�� �����!", "��� ��������!", "����!" };
                string mes = figurs[sluchai.Next(figurs.Count)];
                UpdateItog($"{mes}", Brushes.Crimson);
            }
        }
    }


    void Raskidka() // ��������� ������ � ��������� �����
    {
        var figurs = new List<String> { "�������", "4-� ��������", "����" };
        Vibronoe = figurs[sluchai.Next(figurs.Count)];
        var Sluchainost = new Point(sluchai.Next(0, (int)Pole.Bounds.Width), sluchai.Next(0, (int)Pole.Bounds.Height));
        Otrisovka(Sluchainost);
    }

  /*  
      void RaskidkaPoFiguram()
    {
        Vibronoe = "�������";
        var SluchainostKvadro = new Point(sluchai.Next(0, (int)Pole.Bounds.Width), sluchai.Next(0, (int)Pole.Bounds.Height));
        Otrisovka(SluchainostKvadro);

        Vibronoe = "4-� ��������";
        var SluchainostUgolnik = new Point(sluchai.Next(0, (int)Pole.Bounds.Width), sluchai.Next(0, (int)Pole.Bounds.Height));

        Otrisovka(SluchainostUgolnik);
        Vibronoe = "����";
        var SluchainostRomb = new Point(sluchai.Next(0, (int)Pole.Bounds.Width), sluchai.Next(0, (int)Pole.Bounds.Height));
        Otrisovka(SluchainostRomb);
    }
    */
}