namespace Spider.Core;

internal sealed class SpiderLogic
{
    public int Hunger
    {
        get => _hunger;
        private set => _hunger = Math.Max(0, Math.Min(100, value));
    }
    private int _hunger = 0;

    public SpiderLogic()
    {
        
    }

    public void AddHunger(int amount)
    {
        Hunger += amount;
    }

    public void RemoveHunger(int amount)
    {
        Hunger -= amount;
    }
}