using small_n_stats_WPF.ViewModels;
using small_n_stats_WPF.Views;
using System.Windows;

namespace small_n_stats_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow();
            window.DataContext = new MainWindowViewModel()
            {
                _interface = window,
                MainWindow = window,
            };
            window.Show();
        }
    }
}
