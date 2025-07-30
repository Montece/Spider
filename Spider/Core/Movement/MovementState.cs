namespace Spider.Core.Movement;

internal abstract class MovementState
{
    public abstract void FrameUpdate(SpiderLogic logic);
}