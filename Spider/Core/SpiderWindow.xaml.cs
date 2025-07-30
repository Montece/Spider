using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using NLog;
using Spider.ConfigData;
using Spider.InstanceData;
using Spider.Utility;

namespace Spider.Core;

public sealed partial class SpiderWindow
{
    public SpiderCore SpiderCore { get; }
    
    private readonly Random _random = new();
    private readonly DispatcherTimer _frameTimer = new();

    public SpiderWindow()
    {
        InitializeComponent();

        ConfigManager.Load();
        InstanceManager.Load();

        var spiderPosition = InstanceManager.Instance.SpiderPosition;

        if (spiderPosition == new Point(0, 0))
        {
            spiderPosition = new(500, 500);
        }

        SpiderCore = new(this, SpiderCanvas, spiderPosition);
        SpiderCore.OnSpiderClick += BodyOnMouseLeftButtonDown;

        _frameTimer.Interval = TimeSpan.FromMilliseconds(10);
        _frameTimer.Tick += FrameUpdate;
        _frameTimer.Start();
    }

    private void BodyOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        SpiderCore.Fear(TimeSpan.FromSeconds(3));
    }

    private void FrameUpdate(object? sender, EventArgs e)
    {
        SpiderCore.FrameUpdate();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        HideWindowsIcon();
    }

    private void HideWindowsIcon()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
        var extendedStyle = Win32.GetWindowLong(hwnd, Win32.GWL_EXSTYLE);
        Win32.SetWindowLong(hwnd, Win32.GWL_EXSTYLE, extendedStyle | Win32.WS_EX_TOOLWINDOW);
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private async void Window_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return;
        }

        var files = (string[])e.Data.GetData(DataFormats.FileDrop)!;

        foreach (var file in files)
        {
            await HandleFile(file);
        }
    }

    private async Task HandleFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            return;
        }

        var filename = Path.GetFileName(filepath);
        var fileExtension = Path.GetExtension(filepath).ToLower()[1..];
        var fileSizeInMegaBytes = new FileInfo(filepath).Length / 1024 / 1024;

        Log.Write($"Паук съел файл '{filepath}' весом {fileSizeInMegaBytes} МБ", LogLevel.Info);

        var amountLong = fileSizeInMegaBytes / 50 / 100;
        var amount = amountLong > 100 ? 100 : (int)amountLong;

        SpiderCore.Feed(amount);

        WindowsUtility.DeleteFileAsync(filepath);

        switch (fileExtension)
        {
            case "txt":
            case "log":
                var fileLines = await File.ReadAllLinesAsync(filepath);
                var randomLine = fileLines[_random.Next(0, fileLines.Length)];
                SpiderCore.Say(randomLine);
                break;
            case "mp3":
            case "wav":
            case "ogg":
            case "aac":
            case "m4a":
            case "wma":
            case "opus":
            case "flac":
            case "alac":
            case "ape":
            case "wv":
            case "aiff":
            case "aif":
            case "bwf":
            {
                var title = MediaFileUtility.GetTitle(filepath);
                var artist = MediaFileUtility.GetArtist(filepath);

                if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(artist))
                {
                    switch (_random.Next(0, 3 + 1))
                    {
                        case 0:
                            SpiderCore.Say($"У меня теперь в голове играет {title} от {artist}");
                            break;
                        case 1:
                            SpiderCore.Say($"Знал я пауков из группы {artist}");
                            break;
                        case 2:
                            SpiderCore.Say($"Мне нравится трек {title}'");
                            break;
                        case 3:
                            SpiderCore.Say($"Мне не нравится трек {title}");
                            break;
                    }
                }
                else
                {
                    SayDefault();
                }
                break;
            }
            case "mp4":
            case "mkv":
            case "avi":
            case "mov":
            case "wmv":
            case "webm":
            {
                var title = MediaFileUtility.GetTitle(filepath);
                SpiderCore.Say($"Смотрю фильм {title}");
                break;
            }
            default:
                SayDefault();
                break;
        }

        return;

        void SayDefault()
        {
            SpiderCore.Say($"Я съел файл {filename}");
        }
    }
}