using Spider.Core.Movement;
using Spider.Core.Trashing;
using Spider.InstanceData;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;

namespace Spider.Core;

internal sealed class SpiderLogic : IFrameUpdate
{
    public event Action<int>? OnHungerChanged;

    public int Hunger
    {
        get
        {
            return InstanceManager.Instance.Hunger;
        }
        private set
        {
            InstanceManager.Instance.Hunger = Math.Max(0, Math.Min(100, value));
            InstanceManager.Save();
        }
    }

    public Point Position { get; set; }
    public bool IsFearMode { get; private set; }
    public Window Window { get; }

    private readonly SpiderMouth _mouth;
    private readonly SpiderTrashing _trashing;

    private readonly TimeSpan _hungerTime = TimeSpan.FromMinutes(15);
    private readonly TimeSpan _trashTime = TimeSpan.FromMinutes(15);

    private MovementState _currentMovementState;

    public SpiderLogic(Window window, double startX, double startY)
    {
        Window = window;
        _mouth = new(this);
        _trashing = new(this);

        Position = new(startX, startY);

        _currentMovementState = new Walking(this);

        Task.Run(HungerCycle);
        Task.Run(TrashCycle);
    }

    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    private async void HungerCycle()
    {
        while (true)
        {
            await Task.Delay(_hungerTime);

            AddHunger(1);

            OnHungerChanged?.Invoke(Hunger);
        }
    }

    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    private async void TrashCycle()
    {
        while (true)
        {
            await Task.Delay(_trashTime);

            _trashing.DropTrash(Position);
        }
    }

    private void AddHunger(int amount)
    {
        Hunger += amount;
    }

    public void RemoveHunger(int amount)
    {
        Hunger -= amount;
    }

    public void Fear(TimeSpan fearDuration)
    {
        if (IsFearMode)
        {
            return;
        }

        IsFearMode = true;

        var previousMovementState = _currentMovementState;

        _currentMovementState = new FearWalking(this);
        
        var timer = new DispatcherTimer
        {
            Interval = fearDuration
        };

        timer.Tick += (_, _) =>
        {
            timer.Stop();

            IsFearMode = false;

            _currentMovementState = previousMovementState;
        };

        _mouth.Say("Опять меня тыкают");

        timer.Start();
    }

    public void Say(string text)
    {
        _mouth.Say(text);
    }

    public void FrameUpdate()
    {
        _mouth.FrameUpdate();
        _currentMovementState.FrameUpdate(this);

        InstanceManager.Instance.SpiderPosition = Position;
    }
}