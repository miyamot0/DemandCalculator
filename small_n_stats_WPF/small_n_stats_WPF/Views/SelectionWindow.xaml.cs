using System.Windows;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        public SelectionWindow(string[] options, string defaultItem)
        {
            InitializeComponent();

            foreach (string str in options)
            {
                MessageOptions.Items.Add(str);
            }

            MessageOptions.SelectedItem = defaultItem;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
