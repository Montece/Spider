using System.Windows.Threading;

namespace Spider.Core;

internal sealed class SpiderMouth : IFrameUpdate
{
    private bool IsTalking { get; set; }

    private readonly SpiderLogic _logic;
    private readonly object _speechWindowLock = new();

    private SpeechWindow? _speechWindow;
    private DispatcherTimer? _timer;

    public SpiderMouth(SpiderLogic logic)
    {
        _logic = logic;
    }

    public void Say(string text)
    {
        if (IsTalking)
        {
            StopSaying();
        }

        StartSaying(text);
    }

    private void StartSaying(string text)
    {
        if (IsTalking)
        {
            return;
        }

        IsTalking = true;

        lock (_speechWindowLock)
        {
            _speechWindow = new();
            _speechWindow.SetText(text);
            _speechWindow.Show();
        }

        _timer = new()
        {
            Interval = TimeSpan.FromSeconds(2)
        };

        _timer.Tick += (_, _) =>
        {
            StopSaying();
        };

        _timer.Start();
    }

    private void StopSaying()
    {
        if (!IsTalking)
        {
            return;
        }

        _timer?.Stop();
        _timer = null;

        lock (_speechWindowLock)
        {
            _speechWindow?.Close();
            _speechWindow = null;
        }

        IsTalking = false;
    }

    public void FrameUpdate()
    {
        lock (_speechWindowLock)
        {
            _speechWindow?.Follow(_logic.Position);
        }
    }
}