using System.Windows;

namespace Spider;

public partial class SpeechWindow
{
    public SpeechWindow()
    {
        InitializeComponent();
    }

    public void SetText(string text)
    {
        SpeechText.Text = text;
    }

    public void Follow(Point spiderScreenPos)
    {
        Left = spiderScreenPos.X + 10;
        Top = spiderScreenPos.Y - Height - 10;
    }
}