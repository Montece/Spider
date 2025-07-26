using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Spider.Core;

internal sealed class SpiderCore
{
    public event MouseButtonEventHandler OnSpiderClick;

    private readonly SpiderLogic _logic;
    private readonly SpiderView _view;
    private readonly SpiderMouth _mouth;

    public SpiderCore(Window window, Canvas canvas)
    {
        _view = new(window, canvas);
        _logic = new();
        _mouth = new(this);

        _view.OnBodyMouseLeftButtonDown += (sender, args) => OnSpiderClick?.Invoke(sender, args);
    }

    public Point GetSpiderPosition() => _view.GetSpiderPosition();

    public void SetSpiderPosition(Point newPosition) => _view.SetSpiderPosition(newPosition);

    public void Speak(string text) => _mouth.Speak(text);

    public void UpdateEyes(int hunger) => _view.UpdateEyes(hunger);

    public void AnimateLegs() => _view.AnimateLegs();

    public void Hungry(int amount)
    {
        _logic.AddHunger(amount);
        _view.UpdateEyes(_logic.Hunger);
    }

    public void Feed(int amount)
    {
        _logic.RemoveHunger(amount);
        _view.UpdateEyes(_logic.Hunger);
    }
}