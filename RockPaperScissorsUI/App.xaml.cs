using System.Windows;

namespace RockPaperScissorsUI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var mainViewModel = new MainViewModel();
        var window = new MainWindow()
        {
            DataContext = mainViewModel
        };

        window.Show();
    }
}
