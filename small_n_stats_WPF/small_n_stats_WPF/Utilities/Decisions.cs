//----------------------------------------------------------------------------------------------
// <copyright file="Decisions.cs" 
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

using small_n_stats_WPF.Views;
using System.Windows;

namespace small_n_stats_WPF.Utilities
{
    /// <summary>
    /// Enum values to aide in automated decision-making.
    /// </summary>
    public enum YValueDecisions
    {
        DoNothing,
        DropZeros,
        ChangeTenth,
        ChangeHundredth,
        ChangeCustom,
        OnePercentLowest
    }

    /// <summary>
    /// Enum values to aide in automated decision-making.
    /// </summary>
    public enum XValueDecisions
    {
        DoNothing,
        ChangeHundredth,
        DropZeros
    }

    /// <summary>
    /// Enum values to aide in automated decision-making.  
    /// </summary>
    public enum KValueDecisions
    {
        DeriveValuesIndividual,
        DeriveValuesGroup,
        AverageLogValuesGroup,
        UseSuppliedValues,
        FitK
    }

    public class Decisions
    {
        /// <summary>
        /// Supplies description of behavior
        /// </summary>
        /// <param name="decision"></param>
        /// <returns></returns>
        public static string GetYBehaviorDescription(YValueDecisions decision)
        {
            string holderString = string.Empty;

            switch (decision)
            {
                case YValueDecisions.ChangeCustom:
                    holderString = "Zero Y values : Custom Value ";
                    break;

                case YValueDecisions.ChangeTenth:
                    holderString = "Zero Y values : Changed to 0.1";
                    break;

                case YValueDecisions.ChangeHundredth:
                    holderString = "Zero Y values : Changed to 0.01";
                    break;

                case YValueDecisions.DoNothing:
                    holderString = "Zero Y Values : Unmodified";
                    break;

                case YValueDecisions.OnePercentLowest:
                    holderString = "Zero Y Values : 1% of lowest value";
                    break;

                case YValueDecisions.DropZeros:
                    holderString = "Zero Y Values : Dropped";
                    break;
            }

            return holderString;
        }

        /// <summary>
        /// Supplies description of behavior
        /// </summary>
        /// <param name="decision"></param>
        /// <returns></returns>
        public static string GetXBehaviorDescription(XValueDecisions decision)
        {
            string holderString = string.Empty;

            switch (decision)
            {
                case XValueDecisions.ChangeHundredth:
                    holderString = "Zero X values : Changed to 0.01";
                    break;

                case XValueDecisions.DoNothing:
                    holderString = "Zero X values : Unmodified";
                    break;

                case XValueDecisions.DropZeros:
                    holderString = "Zero X values : Dropped";
                    break;
            }

            return holderString;
        }

        /// <summary>
        /// Supplies description of behavior
        /// </summary>
        /// <param name="decision"></param>
        /// <returns></returns>
        public static string GetKBehaviorDescription(KValueDecisions decision)
        {
            string holderString = string.Empty;

            switch(decision)
            {
                case KValueDecisions.DeriveValuesIndividual:
                    holderString = "K values : Derived from individual series";
                    break;

                case KValueDecisions.DeriveValuesGroup:
                    holderString = "K values : Derived from whole sample";
                    break;

                case KValueDecisions.AverageLogValuesGroup:
                    holderString = "K values : Averaged Empirical Logs";
                    break;

                case KValueDecisions.UseSuppliedValues:
                    holderString = "K values : User supplied K";
                    break;

                case KValueDecisions.FitK:
                    holderString = "K values : Fitted with other parameters";
                    break;
            }

            return holderString;
        }

        /// <summary>
        /// Query's user about how to address certain values
        /// </summary>
        /// <param name="modelType">
        /// modelType informs the "default", recommended option
        /// </param>
        /// <returns>
        /// Decision enum
        /// </returns>
        public static YValueDecisions GetYBehavior(string modelType, Window windowRef)
        {
            string recommended = (modelType == "Exponential") ? "Drop Zeroes" : "Do Nothing";

            var yValueWindow = new SelectionWindow(new string[] { "Drop Zeroes", "Change Hundredth", "Change Tenth", "Custom Value", "Do Nothing" }, recommended);
            yValueWindow.Title = "How do you want to treat 0 Consumption values";
            yValueWindow.MessageLabel.Text = "Please select how to manage the zero Y values";
            yValueWindow.Owner = windowRef;
            yValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            yValueWindow.Topmost = true;

            if (yValueWindow.ShowDialog() == true)
            {
                int output = yValueWindow.MessageOptions.SelectedIndex;

                if (output == 0)
                {
                    return YValueDecisions.DropZeros;
                }
                else if (output == 1)
                {
                    return YValueDecisions.ChangeHundredth;
                }
                else if (output == 2)
                {
                    return YValueDecisions.ChangeTenth;
                }
                else if (output == 3)
                {
                    return YValueDecisions.ChangeCustom;
                }
                else if (output == 4)
                {
                    return YValueDecisions.DoNothing;
                }
            }

            return YValueDecisions.DoNothing;
        }

        /// <summary>
        /// Query's user about how to address certain values
        /// </summary>
        /// <param name="modelType">
        /// modelType informs the "default", recommended option
        /// </param>
        /// <returns>
        /// Decision enum
        /// </returns>
        public static YValueDecisions GetYBehavior(bool isHursh, Window windowRef)
        {
            string recommended = (isHursh) ? "Drop Zeroes" : "Do Nothing";

            var yValueWindow = new SelectionWindow(new string[] { "Drop Zeroes", "Change Hundredth", "Change Tenth", "Custom Value", "Do Nothing" }, recommended);
            yValueWindow.Title = "How do you want to treat 0 Consumption values";
            yValueWindow.MessageLabel.Text = "Please select how to manage the zero Y values";
            yValueWindow.Owner = windowRef;
            yValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            yValueWindow.Topmost = true;

            if (yValueWindow.ShowDialog() == true)
            {
                int output = yValueWindow.MessageOptions.SelectedIndex;

                if (output == 0)
                {
                    return YValueDecisions.DropZeros;
                }
                else if (output == 1)
                {
                    return YValueDecisions.ChangeHundredth;
                }
                else if (output == 2)
                {
                    return YValueDecisions.ChangeTenth;
                }
                else if (output == 3)
                {
                    return YValueDecisions.ChangeCustom;
                }
                else if (output == 4)
                {
                    return YValueDecisions.DoNothing;
                }
            }

            return YValueDecisions.DoNothing;
        }

        /// <summary>
        /// Query's user about how to address certain values
        /// </summary>
        /// <param name="modelType">
        /// modelType informs the "default", recommended option
        /// </param>
        /// <returns>
        /// Decision enum
        /// </returns>
        public static XValueDecisions GetXBehavior(string modelType, Window windowRef)
        {
            string recommended = (modelType == "Exponential") ? "Drop Zeroes" : "Do Nothing";

            var xValueWindow = new SelectionWindow(new string[] { "Drop Zeroes", "Do Nothing" }, recommended);
            xValueWindow.Title = "How do you want to treat 0 Pricing (free) values";
            xValueWindow.MessageLabel.Text = "Please select how to manage the zero X values";
            xValueWindow.Owner = windowRef;
            xValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            xValueWindow.Topmost = true;

            if (xValueWindow.ShowDialog() == true)
            {
                int output = xValueWindow.MessageOptions.SelectedIndex;

                if (output == 0)
                {
                    return XValueDecisions.DropZeros;
                }
                else if (output == 1)
                {
                    return XValueDecisions.DoNothing;
                }
            }

            return XValueDecisions.DoNothing;
        }

        /// <summary>
        /// Query's user about how to address certain values
        /// </summary>
        /// <param name="modelType">
        /// modelType informs the "default", recommended option
        /// </param>
        /// <returns>
        /// Decision enum
        /// </returns>
        public static XValueDecisions GetXBehavior(bool isHursh, Window windowRef)
        {
            string recommended = (isHursh) ? "Drop Zeroes" : "Do Nothing";

            var xValueWindow = new SelectionWindow(new string[] { "Drop Zeroes", "Do Nothing" }, recommended);
            xValueWindow.Title = "How do you want to treat 0 Pricing (free) values";
            xValueWindow.MessageLabel.Text = "Please select how to manage the zero X values";
            xValueWindow.Owner = windowRef;
            xValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            xValueWindow.Topmost = true;

            if (xValueWindow.ShowDialog() == true)
            {
                int output = xValueWindow.MessageOptions.SelectedIndex;

                if (output == 0)
                {
                    return XValueDecisions.DropZeros;
                }
                else if (output == 1)
                {
                    return XValueDecisions.DoNothing;
                }
            }

            return XValueDecisions.DoNothing;
        }

        /// <summary>
        /// Query's user about how to address certain values
        /// </summary>
        /// <param name="modelType">
        /// modelType informs the "default", recommended option
        /// </param>
        /// <returns>
        /// Decision enum
        /// </returns>
        public static KValueDecisions GetKBehaviorIndividual(Window windowRef)
        {
            var kValueWindow = new SelectionWindow(new string[] { "Use derived K (individual)", "Fit a K (individual)", "Use Custom Ks" }, "Use derived K (individual)");
            kValueWindow.Title = "How do you want to derive K values";
            kValueWindow.MessageLabel.Text = "Please select how to ascertain K:";
            kValueWindow.Owner = windowRef;
            kValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            kValueWindow.Topmost = true;

            if (kValueWindow.ShowDialog() == true)
            {
                int output = kValueWindow.MessageOptions.SelectedIndex;

                if (output == 0)
                {
                    return KValueDecisions.DeriveValuesIndividual;
                }
                else if (output == 1)
                {
                    return KValueDecisions.FitK;
                }
                else if (output == 2)
                {
                    return KValueDecisions.UseSuppliedValues;
                }
            }

            return KValueDecisions.DeriveValuesIndividual;
        }

        /// <summary>
        /// Query's user about how to address certain values
        /// </summary>
        /// <param name="modelType">
        /// modelType informs the "default", recommended option
        /// </param>
        /// <returns>
        /// Decision enum
        /// </returns>
        public static KValueDecisions GetKBehaviorGroup(Window windowRef)
        {
            var kValueWindow = new SelectionWindow(new string[] { "Fit K as parameter", "Use derived K (group)", "Use derived K (individual)", "Average derived K", "Use Custom Ks" }, 
                "Use derived K (group)");
            kValueWindow.Title = "How do you want to derive K values";
            kValueWindow.MessageLabel.Text = "Please select how to ascertain K:";
            kValueWindow.Owner = windowRef;
            kValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            kValueWindow.Topmost = true;

            if (kValueWindow.ShowDialog() == true)
            {
                int output = kValueWindow.MessageOptions.SelectedIndex;

                if (output == 0)
                {
                    return KValueDecisions.FitK;
                }
                else if (output == 1)
                {
                    return KValueDecisions.DeriveValuesGroup;
                }
                else if (output == 2)
                {
                    return KValueDecisions.DeriveValuesIndividual;
                }
                else if (output == 3)
                {
                    return KValueDecisions.AverageLogValuesGroup;
                }
                else if (output == 4)
                {
                    return KValueDecisions.UseSuppliedValues;
                }
            }

            return KValueDecisions.DeriveValuesGroup;
        }

    }
}
