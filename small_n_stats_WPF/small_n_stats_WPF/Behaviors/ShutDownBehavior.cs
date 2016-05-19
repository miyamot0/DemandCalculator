/* 
    Copyright 2016 Shawn Gilroy

    This file is part of Demand Analysis.

    Demand Analysis is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    Demand Analysis is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Demand Analysis.  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>.

*/

using System.Windows;
using System.Windows.Interactivity;

namespace small_n_stats_WPF.Behaviors
{
    class ShutDownBehavior : Behavior<Window>
    {
        /// <summary>
        /// Event behavior (Window) for window close events, attached to main window primarily for menuitem close command
        /// </summary>
        public static readonly DependencyProperty ShutDownProperty =
            DependencyProperty.Register("ShuttingDownFlag",
                typeof(bool), typeof(ShutDownBehavior),
                new PropertyMetadata(false, OnValueChanged));

        /// <summary>
        /// Behavior reference value 
        /// </summary>
        public bool ShuttingDownFlag
        {
            get { return (bool)GetValue(ShutDownProperty); }
            set
            {
                SetValue(ShutDownProperty, value);
                
                /* Test! Might be able to avoid the event all together. potentially null out propter metadata?
                    if (value == true)
                    {
                        ((Window)AssociatedObject).Close();
                    }
                */
            }
        }

        /// <summary>
        /// Event trigger, calling non-static private method 
        /// </summary>
        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((ShutDownBehavior)obj).OnValueChanged();
        }

        /// <summary>
        /// Called method on change, call close action if changed value is false (i.e., window open status = shutting down flag)
        /// </summary>
        private void OnValueChanged()
        {
            if (ShuttingDownFlag)
            {
                ((Window)AssociatedObject).Close();
            }
        }
    }
}
