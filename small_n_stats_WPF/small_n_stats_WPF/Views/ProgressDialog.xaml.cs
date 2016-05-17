using System.Windows;

namespace small_n_stats_WPF.ViewModels
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class ProgressDialog : Window
    {
        public ProgressDialog(string text, string title)
        {
            InitializeComponent();
            ProgressText.Text = text;
            Title = title;
        }
    }
}
