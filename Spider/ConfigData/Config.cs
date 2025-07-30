using JetBrains.Annotations;

namespace Spider.ConfigData;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class Config
{
    public double Speed { get; set; } = 1d;
    public bool StartWithWindows { get; set; }
    public bool OnlyMainScreen { get; set; }
}