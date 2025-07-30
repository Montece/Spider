namespace Spider.Core.Trashing;

public partial class TrashWindow
{
    public TrashWindow(Trash trash)
    {
        InitializeComponent();

        Tag = trash.WindowGuid;
        Left = trash.Position.X;
        Top = trash.Position.Y;
    }
}