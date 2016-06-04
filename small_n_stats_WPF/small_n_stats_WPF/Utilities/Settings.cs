//----------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" 
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

namespace small_n_stats_WPF.Properties
{
    internal sealed partial class Settings
    {
        /// <summary>
        /// Base settings access class
        /// </summary>
        public Settings() { }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e) { }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e) { }

    }
}
