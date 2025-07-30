using System.Windows;
using Spider.Utility;

namespace Spider.Core.Movement;

internal sealed class FearWalking : Walking
{
    public FearWalking(SpiderLogic logic) : base(logic)
    {
        var spiderDirection = GetSpiderDirection(logic.Position);
        
        spiderDirection.Normalize();
        spiderDirection = InverseDirection(spiderDirection);
            
        var newTarget = new Point(logic.Position.X + spiderDirection.X * 300, logic.Position.Y + spiderDirection.Y * 300);
        var currentTarget = ClampToScreenBounds(newTarget);

        SetTarget(currentTarget);
    }

    private Vector InverseDirection(Vector direction)
    {
        return -direction;
    }

    private Point ClampToScreenBounds(Point point)
    {
        var (screenWidth, screenHeight) = WindowsUtility.GetScreenBounds();

        var x = Math.Max(0, Math.Min(screenWidth - 100, point.X));
        var y = Math.Max(0, Math.Min(screenHeight - 100, point.Y));

        return new(x, y);
    }
}