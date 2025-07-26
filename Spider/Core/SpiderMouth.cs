using System.Windows;
using System.Windows.Threading;

namespace Spider.Core;

internal sealed class SpiderMouth
{
    private bool IsSpeaking { get; set; }

    private readonly SpiderCore _core;

    public SpiderMouth(SpiderCore core)
    {
        _core = core;
    }

    public void Speak(string text)
    {
        if (IsSpeaking)
        {
            return;
        }

        IsSpeaking = true;

        var speech = new SpeechWindow();
        speech.SetText(text);
        speech.Show();

        var timer1 = new DispatcherTimer
        {
            Interval = TimeSpan.FromMicroseconds(30)
        };

        timer1.Tick += (_, _) =>
        {
            speech.Follow(_core.GetSpiderPosition());
        };

        timer1.Start();

        var timer2 = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };

        timer2.Tick += (_, _) =>
        {
            timer1.Stop();
            timer2.Stop();

            speech.Close();

            IsSpeaking = false;
        };

        timer2.Start();
    }
}