using Hardcodet.Wpf.TaskbarNotification;
using Spider.Core;
using Spider.InstanceData;
using System.Windows;

namespace Spider;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var trayIcon = GetIcon();
            trayIcon.TrayLeftMouseDown += (_, _) =>
            {
                if (MainWindow!.IsVisible)
                {
                    MainWindow.Hide();
                }
                else
                {
                    MainWindow.Show();
                }
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка иконки в трее: " + ex.Message);
        }
    }

    private TaskbarIcon GetIcon()
    {
        return (TaskbarIcon)(FindResource("MyTrayIcon") ?? throw new InvalidOperationException());
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        new SettingsWindow().Show();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var trayIcon = GetIcon();
        trayIcon.Dispose();

        InstanceManager.Save();

        base.OnExit(e);
    }
}