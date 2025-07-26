using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Spider.Core;

internal sealed class SpiderView
{
    public event MouseButtonEventHandler OnBodyMouseLeftButtonDown;

    private const int LEG_COUNT = 8;
    private const double LEG_LENGTH = 20d;
    private const double LEG_THICKNESS = 2d;

    private const double BODY_RADIUS = 10d;
    private const double EYE_SIZE = 6d;
    private const double EYE_OFFSET_X = 4d;
    private const double EYE_OFFSET_Y = -2d;

    private readonly Brush _legColor = Brushes.LightGray;
    private readonly Brush _bodyColor = Brushes.DarkSlateGray;

    private readonly Window _window;
    private readonly Canvas _canvas;
    private readonly Line[] _legs = new Line[LEG_COUNT];

    private Ellipse _body;
    private Ellipse _eyeLeft;
    private Ellipse _eyeRight;

    private double _time;

    public SpiderView(Window window, Canvas canvas)
    {
        _window = window;
        _canvas = canvas;

        DrawSpider();
    }

    private void DrawSpider()
    {
        // Лапки
        for (var i = 0; i < LEG_COUNT; i++)
        {
            var leg = new Line
            {
                Stroke = _legColor,
                StrokeThickness = LEG_THICKNESS
            };
            _legs[i] = leg;
            _canvas.Children.Add(leg);
        }

        // Тело
        _body = new()
        {
            Width = BODY_RADIUS * 2,
            Height = BODY_RADIUS * 2,
            Fill = _bodyColor,
            Cursor = Cursors.Hand
        };

        _canvas.Children.Add(_body);
        
        _body.MouseLeftButtonDown += (s, e) => OnBodyMouseLeftButtonDown?.Invoke(s, e);

        // Глаза
        _eyeLeft = CreateEye();
        _eyeRight = CreateEye();
        _canvas.Children.Add(_eyeLeft);
        _canvas.Children.Add(_eyeRight);

        SetBodyPosition((_canvas.Width - _body.Width) / 2, (_canvas.Height - _body.Height) / 2);
    }

    private static Ellipse CreateEye()
    {
        return new()
        {
            Width = EYE_SIZE,
            Height = EYE_SIZE,
            Fill = Brushes.White,
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            IsHitTestVisible = false
        };
    }

    private void SetBodyPosition(double x, double y)
    {
        Canvas.SetLeft(_body, x);
        Canvas.SetTop(_body, y);

        UpdateEyePositions();
    }

    private Point GetBodyCenter()
    {
        var x = Canvas.GetLeft(_body) + _body.Width / 2;
        var y = Canvas.GetTop(_body) + _body.Height / 2;

        return new(x, y);
    }

    private void UpdateEyePositions()
    {
        var center = GetBodyCenter();

        Canvas.SetLeft(_eyeLeft, center.X - EYE_OFFSET_X - EYE_SIZE / 2);
        Canvas.SetTop(_eyeLeft, center.Y + EYE_OFFSET_Y - EYE_SIZE / 2);

        Canvas.SetLeft(_eyeRight, center.X + EYE_OFFSET_X - EYE_SIZE / 2);
        Canvas.SetTop(_eyeRight, center.Y + EYE_OFFSET_Y - EYE_SIZE / 2);
    }

    public void AnimateLegs()
    {
        _time += 0.1;

        var center = GetBodyCenter();
        var angleStep = Math.PI / (LEG_COUNT / 2d);

        for (var i = 0; i < LEG_COUNT; i++)
        {
            var angle = i * angleStep + Math.Sin(_time + i) * 0.3;
            var x1 = center.X;
            var y1 = center.Y;
            var x2 = x1 + Math.Cos(angle) * LEG_LENGTH;
            var y2 = y1 + Math.Sin(angle) * LEG_LENGTH;

            _legs[i].X1 = x1;
            _legs[i].Y1 = y1;
            _legs[i].X2 = x2;
            _legs[i].Y2 = y2;
        }
    }

    public void UpdateEyes(int hunger)
    {
        Brush color = hunger switch
        {
            < 30 => Brushes.LightGreen,
            < 70 => Brushes.Yellow,
            _ => Brushes.IndianRed
        };

        _eyeLeft.Fill = _eyeRight.Fill = color;
    }

    public void SetSpiderPosition(Point newPosition)
    {
        _window.Left = newPosition.X;
        _window.Top = newPosition.Y;
    }

    public Point GetSpiderPosition()
    {
        return new(_window.Left, _window.Top);
    }
}