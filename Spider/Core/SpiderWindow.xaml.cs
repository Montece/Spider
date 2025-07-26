using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using NLog;

namespace Spider.Core;

public sealed partial class SpiderWindow
{
    private readonly Random _random = new();
    private readonly DispatcherTimer _frameTimer = new();
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly SpiderCore _spiderCore;

    private bool _fearMode;

    private double _currentX = 500;
    private double _currentY = 500;
    private double _targetX;
    private double _targetY;

    public SpiderWindow()
    {
        InitializeComponent();

        ConfigManager.Load();

        _spiderCore = new(this, SpiderCanvas);
        _spiderCore.OnSpiderClick += BodyOnMouseLeftButtonDown;

        _spiderCore.SetSpiderPosition(new(_currentX, _currentY));

        PickNewTarget();

        _frameTimer.Interval = TimeSpan.FromMilliseconds(10);
        _frameTimer.Tick += FrameUpdate;
        _frameTimer.Start();

        Task.Run(HungerCycle);
    }

    private void BodyOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_fearMode)
        {
            return;
        }

        _spiderCore.Speak("Я хочу кушать");

        var spiderPosition = _spiderCore.GetSpiderPosition();
        var dir = new Vector(_targetX - spiderPosition.X, spiderPosition.Y - _targetY);
        
        if (dir.Length > 0.01)
        {
            dir.Normalize();

            var newDir = -dir; // Повернуть — взять противоположный вектор
            
            var newTarget = new Point(spiderPosition.X + newDir.X * 300, spiderPosition.Y + newDir.Y * 300);

            var currentTarget = ClampToScreenBounds(newTarget);
            _targetX = currentTarget.X;
            _targetY = currentTarget.Y;

            // Увеличить скорость временно
            _fearMode = true;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                _fearMode = false;
                ((DispatcherTimer)s).Stop();
            };
            timer.Start();
        }
    }

    private void FrameUpdate(object? sender, EventArgs e)
    {
        _spiderCore.AnimateLegs();

        var dx = _targetX - _currentX;
        var dy = _targetY - _currentY;
        var distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance < 5)
        {
            PickNewTarget();
        }
        else
        {
            var speed = ConfigManager.Instance.Speed;
            speed *= _fearMode ? 10d : 1d; 
            var moveX = dx / distance * speed;
            var moveY = dy / distance * speed;
            _currentX += moveX;
            _currentY += moveY;

            _spiderCore.SetSpiderPosition(new(_currentX, _currentY));
        }
    }

    private async void HungerCycle()
    {
        while (true)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            Dispatcher.Invoke(() => _spiderCore.Hungry(1));
        }
    }

    private void PickNewTarget()
    {
        var (screenWidth, screenHeight) = GetScreenBounds();

        _targetX = _random.Next(0, (int)(screenWidth - Width));
        _targetY = _random.Next(0, (int)(screenHeight - Height));
    }

    private (double, double) GetScreenBounds()
    {
        double screenWidth;
        double screenHeight;

        if (ConfigManager.Instance.OnlyMainScreen)
        {
            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;
        }
        else
        {
            screenWidth = SystemParameters.VirtualScreenWidth;
            screenHeight = SystemParameters.VirtualScreenHeight;
        }

        return (screenWidth, screenHeight);
    }

    private Point ClampToScreenBounds(Point p)
    {
        var (screenWidth, screenHeight) = GetScreenBounds();

        var x = Math.Max(0, Math.Min(screenWidth - 100, p.X));
        var y = Math.Max(0, Math.Min(screenHeight - 100, p.Y));

        return new(x, y);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        HideWindowsIcon();
    }

    private void HideWindowsIcon()
    {
        var hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
        var extendedStyle = Win32.GetWindowLong(hwnd, Win32.GWL_EXSTYLE);
        Win32.SetWindowLong(hwnd, Win32.GWL_EXSTYLE, extendedStyle | Win32.WS_EX_TOOLWINDOW);
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return;
        }

        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        foreach (var file in files)
        {
            HandleFedFile(file);
        }
    }

    private void HandleFedFile(string filePath)
    {
        var filename = System.IO.Path.GetFileName(filePath);
        var fileExtension = System.IO.Path.GetExtension(filePath).ToLower();

        _logger.Info($"Паук съел файл '{filename}'");

        if (fileExtension == ".log" || filePath.Contains("bug"))
        {
            _spiderCore.Feed(40);
            //ShowBalloon("Паук съел баг: " + Path.GetFileName(filePath));
        }
        else
        {
            //ShowBalloon("Паук не любит такие файлы: " + Path.GetFileName(filePath));
        }
    }
}