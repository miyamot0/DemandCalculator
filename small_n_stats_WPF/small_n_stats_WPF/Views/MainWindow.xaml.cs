/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * Small n Stats Application
 * Modeled from conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///  OutputEvents - Is passed a stirng value, subsequently passed to RichTextBox
        /// </summary>
        public void OutputEvents(string output)
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add(output);
            outputWindow.Document.Blocks.Add(para);
            outputWindow.ScrollToEnd();
            Scroller.ScrollToEnd();
        }

        /// <summary>
        ///  SaveLogsEvent - Save contents of RichTextBox to .txt file
        /// </summary>
        public void SaveLogs()
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "Logs";
            sd.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";

            if (sd.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(sd.FileName))
                {
                    TextRange textRange = new TextRange(outputWindow.Document.ContentStart, outputWindow.Document.ContentEnd);
                    sw.Write(textRange.Text);
                }
            }
        }

        /// <summary>
        ///  ClearLogsEvent - Clear contents of RichTextBox
        /// </summary>
        public void ClearLogs()
        {
            outputWindow.Document.Blocks.Clear();
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }

        private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Normal)
            {
                WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
