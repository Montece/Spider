using System.Windows;
using Spider.InstanceData;

namespace Spider.Core.Trashing;

internal sealed class SpiderTrashing
{
    private const int MAX_TRASH_COUNT = 10;

    private readonly SpiderLogic _logic;
    private readonly Dictionary<Guid, TrashWindow> _trashWindowsPool = new();

    public SpiderTrashing(SpiderLogic logic)
    {
        _logic = logic;

        foreach (var trash in InstanceManager.Instance.Trash)
        {
            CreateTrashWindow(trash);
        }
    }

    public void DropTrash(Point position)
    {
        if (InstanceManager.Instance.Trash.Count >= MAX_TRASH_COUNT)
        {
            return;
        }

        var trash = new Trash(Guid.NewGuid(), position);

        CreateTrashWindow(trash);

        InstanceManager.Instance.Trash.Add(trash);
        InstanceManager.Save();
    }

    private void CreateTrashWindow(Trash trash)
    {
        _logic.Window.Dispatcher.Invoke(() =>
        {
            var trashWindow = new TrashWindow(trash);

            trashWindow.MouseLeftButtonDown += (w, _) =>
            {
                _logic.Window.Dispatcher.Invoke(() =>
                {
                    trashWindow.Close();
                });

                var trashWindowGuid = (Guid)((Window)w).Tag;

                InstanceManager.Instance.Trash.RemoveAll(t => t.WindowGuid.Equals(trashWindowGuid));
                InstanceManager.Save();
                _trashWindowsPool.Remove(trashWindowGuid);
            };

            trashWindow.Show();

            _trashWindowsPool.Add(trash.WindowGuid, trashWindow);
        });
        
    }
}