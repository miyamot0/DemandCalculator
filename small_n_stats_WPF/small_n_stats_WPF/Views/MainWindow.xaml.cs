/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using Microsoft.Win32;
using small_n_stats_WPF.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using unvell.ReoGrid.IO;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, SpreadsheetInterface
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void GainFocus()
        {
            spreadSheetView.Focus();
        }

        public bool NewFile()
        {
            spreadSheetView.Reset();
            Title = "Small n Stats - " + "New File";
            return false;
        }

        public string[] OpenFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "XLSX Files|*.xlsx";
            openFileDialog1.Title = "Select an Excel File";

            if (openFileDialog1.ShowDialog() == true)
            {
                using (Stream myStream = openFileDialog1.OpenFile())
                {
                    spreadSheetView.Load(myStream, FileFormat.Excel2007);
                    Title = "Small n Stats - " + openFileDialog1.SafeFileName;
                }

                return new string[] { openFileDialog1.SafeFileName, System.IO.Path.GetDirectoryName(openFileDialog1.FileName) };
            }

            return null;
        }

        public void SaveFile(string path, string title)
        {
            using (Stream myStream = new FileStream(System.IO.Path.Combine(path, title), FileMode.Create))
            {
                var workbook = spreadSheetView;
                workbook.Save(myStream, FileFormat.Excel2007);
                Title = "Small n Stats - " + title;
            }
        }

        public string SaveFileWithDialog(string title)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xls)|*.xls|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                using (Stream myStream = saveFileDialog1.OpenFile())
                {
                    var workbook = spreadSheetView;
                    workbook.Save(myStream, FileFormat.Excel2007);
                    title = saveFileDialog1.SafeFileName;
                    Title = "Small n Stats - " + saveFileDialog1.SafeFileName;
                }

                return saveFileDialog1.SafeFileName;
            }
            else
            {
                return null;
            }

        }

        public string SaveFileAs(string title)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xls)|*.xls|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                using (Stream myStream = saveFileDialog1.OpenFile())
                {
                    var workbook = spreadSheetView;
                    workbook.Save(myStream, FileFormat.Excel2007);
                    title = saveFileDialog1.SafeFileName;
                    Title = "Small n Stats - " + saveFileDialog1.SafeFileName;
                }

                return title;
            }
            else
            {
                return null;
            }

        }

        public void ShutDown()
        {
            Close();
        }

        public void UpdateTitle(string _title)
        {
            Title = "Small n Stats - " + _title;
        }

        public List<double> ParseRange(string range)
        {
            List<double> mReturned = new List<double>();

            try
            {
                var rangeReturned = spreadSheetView.Worksheets[0].Ranges[range];

                spreadSheetView.Worksheets[0].IterateCells(rangeReturned, (row, col, cell) =>
                {
                    double num;
                    if (double.TryParse(cell.Data.ToString(), out num))
                    {
                        mReturned.Add(num);
                    }
                    return true;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return mReturned;
        }

        private void saveLogs_Click(object sender, System.Windows.RoutedEventArgs e)
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

        private void clearLogs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            outputWindow.Document.Blocks.Clear();
        }
    }
}
