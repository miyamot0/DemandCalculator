using System.Windows;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for RangePrompt.xaml
    /// </summary>
    public partial class RangePrompt : Window
    {
        public RangePrompt()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get { return RangeText.Text; }
            set { RangeText.Text = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
