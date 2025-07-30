using Spider.ConfigData;
using Spider.Utility;
using System.Windows;

namespace Spider.Core.Movement;

internal class Walking : MovementState
{
    private const double FEAR_SPEED = 10d;

    private readonly Random _random = new();

    private Point _target;

    public Walking(SpiderLogic logic)
    {
        SetRandomTarget(logic);
    }

    public override void FrameUpdate(SpiderLogic logic)
    {
        var dx = _target.X - logic.Position.X;
        var dy = _target.Y - logic.Position.Y;
        var distance = Math.Sqrt(dx * dx + dy * dy);

        if (distance < 5)
        {
            SetRandomTarget(logic);
        }
        else
        {
            var speed = ConfigManager.Instance.Speed;
            speed *= logic.IsFearMode ? FEAR_SPEED : 1d;
            
            var moveX = dx / distance * speed;
            var moveY = dy / distance * speed;

            logic.Position = new(logic.Position.X + moveX, logic.Position.Y + moveY);
        }
    }

    protected void SetTarget(Point newTarget)
    {
        _target = newTarget;
    }

    private void SetRandomTarget(SpiderLogic logic)
    {
        var (screenWidth, screenHeight) = WindowsUtility.GetScreenBounds();

        var newX = _random.Next(0, (int)(screenWidth - logic.Window.Width));
        var newY = _random.Next(0, (int)(screenHeight - logic.Window.Height));

        SetTarget(new(newX, newY));
    }

    protected Vector GetSpiderDirection(Point spiderPosition)
    {
        var direction = new Vector(_target.X - spiderPosition.X, spiderPosition.Y - _target.Y);

        return direction;
    }
}