using JetBrains.Annotations;
using System.Windows;

namespace Spider.Core.Trashing;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public struct Trash
{
    public Guid WindowGuid { get; set; }
    public Point Position { get; set; }

    public Trash(Guid windowGuid, Point position)
    {
        WindowGuid = windowGuid;
        Position = position;
    }
}