using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace Spider;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var trayIcon = (TaskbarIcon)FindResource("MyTrayIcon");
            trayIcon.TrayLeftMouseDown += (s, ev) =>
            {
                if (MainWindow.IsVisible)
                    MainWindow.Hide();
                else
                    MainWindow.Show();
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка иконки в трее: " + ex.Message);
        }
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
        var trayIcon = (TaskbarIcon)FindResource("MyTrayIcon");
        trayIcon.Dispose();
        base.OnExit(e);
    }
}