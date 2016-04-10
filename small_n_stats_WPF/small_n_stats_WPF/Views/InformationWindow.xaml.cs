/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using System.Diagnostics;
using System.Windows;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : Window
    {
        public InformationWindow()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
