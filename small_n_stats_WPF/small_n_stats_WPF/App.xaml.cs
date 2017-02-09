//----------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" 
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

using small_n_stats_WPF.ViewModels;
using small_n_stats_WPF.Views;
using System.Windows;
using unvell.ReoGrid;

namespace small_n_stats_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow ApplicationWindow;

        private static ReoGridControl workbook;
        public static ReoGridControl Workbook
        {
            get
            {
                return ApplicationWindow.reoGridControl;
            }
            set
            {
                workbook = value;
            }
        }

        public static bool IsSearchingForPick = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ApplicationWindow = new MainWindow();
            ApplicationWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ApplicationWindow.DataContext = new MainWindowViewModel();
            ApplicationWindow.Show();
        }
    }
}
