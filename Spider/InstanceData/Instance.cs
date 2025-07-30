using System.Windows;
using JetBrains.Annotations;
using Spider.Core.Trashing;

namespace Spider.InstanceData;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal sealed class Instance
{
    public int Hunger { get; set; }

    public List<Trash> Trash { get; set; } = [];
    public Point SpiderPosition { get; set; }
}