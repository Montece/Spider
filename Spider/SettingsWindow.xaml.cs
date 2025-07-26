using System.Windows;

namespace Spider;

public partial class SettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();

        ConfigManager.Load();

        SpeedSlider.Value = ConfigManager.Instance.Speed;
        StartupCheck.IsChecked = ConfigManager.Instance.StartWithWindows;
        OnlyMainScreenCheck.IsChecked = ConfigManager.Instance.OnlyMainScreen;

        ApplySettings();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        ConfigManager.Instance.Speed = SpeedSlider.Value;
        ConfigManager.Instance.StartWithWindows = StartupCheck.IsChecked == true;
        ConfigManager.Instance.OnlyMainScreen = OnlyMainScreenCheck.IsChecked == true;

        ConfigManager.Save();

        ApplySettings();

        MessageBox.Show("Настройки сохранены!");

        Close();
    }

    private void ApplySettings()
    {
        if (ConfigManager.Instance.StartWithWindows)
        {
            Startup.AddToStartupFolder();
        }
        else
        {
            Startup.RemoveFromStartupFolder();
        }
    }
}