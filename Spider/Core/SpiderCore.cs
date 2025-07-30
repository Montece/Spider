using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Spider.Core;

public sealed class SpiderCore : IFrameUpdate
{
    public event MouseButtonEventHandler? OnSpiderClick;

    private readonly SpiderLogic _logic;
    private readonly SpiderView _view;

    public SpiderCore(Window window, Canvas canvas, Point startPosition)
    {
        _view = new(window, canvas);
        _logic = new(window, startPosition.X, startPosition.Y);

        _view.UpdateEyes(_logic.Hunger);

        _view.OnBodyMouseLeftButtonDown += (sender, args) => OnSpiderClick?.Invoke(sender, args);
        
        _logic.OnHungerChanged += hunger => _view.UpdateEyes(hunger);
    }

    public void Feed(int amount)
    {
        _logic.RemoveHunger(amount);
    }

    public void Say(string text)
    {
        _logic.Say(text);
    }

    public void FrameUpdate()
    {
        _logic.FrameUpdate();
        _view.FrameUpdate();

        _view.SetPosition(_logic.Position);
    }

    public void Fear(TimeSpan duration)
    {
        _logic.Fear(duration);
    }

    public Point GetPosition()
    {
        return _logic.Position;
    }
}