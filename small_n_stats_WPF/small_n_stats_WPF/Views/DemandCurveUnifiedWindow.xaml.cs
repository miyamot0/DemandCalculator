//----------------------------------------------------------------------------------------------
// <copyright file="DemandCurveUnifiedWindow.cs" 
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
using System.Windows;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for DemandCurveUnifiedWindow.xaml
    /// </summary>
    public partial class DemandCurveUnifiedWindow : Window
    {
        public DemandCurveUnifiedWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (UnifiedDemandCurveViewModel)DataContext;

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
            var viewModel = (UnifiedDemandCurveViewModel)DataContext;

            if (viewModel.ViewClosingCommand.CanExecute(null))
                viewModel.ViewClosingCommand.Execute(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void xRange_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = (UnifiedDemandCurveViewModel)DataContext;

            if (viewModel.PricingRangeCommand.CanExecute(null))
                viewModel.PricingRangeCommand.Execute(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yRange_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = (UnifiedDemandCurveViewModel)DataContext;

            if (viewModel.ConsumptionRangeCommand.CanExecute(null))
                viewModel.ConsumptionRangeCommand.Execute(null);
        }

        private void gRange_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = (UnifiedDemandCurveViewModel)DataContext;

            if (viewModel.GroupingRangeCommand.CanExecute(null))
                viewModel.GroupingRangeCommand.Execute(null);
        }
    }
}
