//----------------------------------------------------------------------------------------------
// <copyright file="MainWindow.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Demand Curve Calculator.
//
// Demand Curve Calculator is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Demand Curve Calculator is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Demand Curve Calculator.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Demand Curve Calculator is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using Microsoft.Win32;
using small_n_stats_WPF.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Documents;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (MainWindowViewModel) DataContext;

            if (viewModel.ViewLoadedCommand.CanExecute(null))
                viewModel.ViewLoadedCommand.Execute(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var viewModel = (MainWindowViewModel) DataContext;

            if (viewModel.ViewClosingCommand.CanExecute(null))
                viewModel.ViewClosingCommand.Execute(null);
        }
    }
}
