//----------------------------------------------------------------------------------------------
// <copyright file="ResizeDialog.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Demand Calculator.
//
// Demand Calculator is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Demand Calculator is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Demand Calculator.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Demand Calculator is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace small_n_stats_WPF.Dialogs
{
    /// <summary>
    /// Interaction logic for YesNoDialog.xaml
    /// </summary>
    public partial class NamingDialog : Window
    {
        /// <summary>
        /// Reference for response
        /// </summary>
        public bool ReturnedAnswer { get; set; }

        /// <summary>
        /// Return for affirm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_Yes(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = true;
            DialogResult = true;
        }

        /// <summary>
        /// Return for decline
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_No(object sender, RoutedEventArgs e)
        {
            ReturnedAnswer = false;
            DialogResult = true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NamingDialog()
        {
            InitializeComponent();
            ReturnedAnswer = false;
        }
    }
}
