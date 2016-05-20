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

    ============================================================================

    R.NET Community is distributed under this license:

    Copyright (c) 2010, RecycleBin
    Copyright (c) 2014-2015 CSIRO

    All rights reserved.

    Redistribution and use in source and binary forms, with or without modification, 
    are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list 
    of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice, this 
    list of conditions and the following disclaimer in the documentation and/or other 
    materials provided with the distribution.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
    ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
    WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
    IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
    INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
    NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
    PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
    WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
    OF SUCH DAMAGE.

*/

using RDotNet;
using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace small_n_stats_WPF.ViewModels
{
    class DemandCurveExponentialViewModel : BaseViewModel
    {
        public MainWindow mWindow { get; set; }
        public DemandCurveWindow windowRef { get; set; }

        private string xRangeValues = "";
        public string XRangeValues
        {
            get { return xRangeValues; }
            set
            {
                xRangeValues = value;
                OnPropertyChanged("XRangeValues");
            }
        }

        private string yRangeValues = "";
        public string YRangeValues
        {
            get { return yRangeValues; }
            set
            {
                yRangeValues = value;
                OnPropertyChanged("YRangeValues");
            }
        }

        private string kValue = "";
        public string KValue
        {
            get { return kValue; }
            set
            {
                kValue = value;
                OnPropertyChanged("KValue");
            }
        }

        private bool advancedMenu = false;
        public bool AdvancedMenu
        {
            get { return advancedMenu; }
            set
            {
                advancedMenu = value;
                OnPropertyChanged("AdvancedMenu");
            }
        }

        private double kValueDouble = 0;

        private Brush xBrush = Brushes.White;
        public Brush XBrush
        {
            get { return xBrush; }
            set
            {
                xBrush = value;
                OnPropertyChanged("XBrush");
            }
        }

        private Brush yBrush = Brushes.White;
        public Brush YBrush
        {
            get { return yBrush; }
            set
            {
                yBrush = value;
                OnPropertyChanged("YBrush");
            }
        }

        private string modelArraySelection;
        public string ModelArraySelection
        {
            get { return modelArraySelection; }
            set
            {
                modelArraySelection = value;
                OnPropertyChanged("ModelArraySelection");
            }
        }

        /* Math */

        REngine engine;

        private bool outputFigures = false;

        bool failed;

        int lowRowX = 0,
            highRowX = 0,
            lowColX = 0,
            highColX = 0;

        int lowRowY = 0,
            highRowY = 0,
            lowColY = 0,
            highColY = 0;

        /* Commands */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand GetXRangeCommand { get; set; }
        public RelayCommand GetYRangeCommand { get; set; }
        public RelayCommand CalculateScoresCommand { get; set; }

        public RelayCommand FigureOutput { get; set; }
        public RelayCommand AdvancedSettings { get; set; }

        public RelayCommand ConsumptionRangeCommand { get; set; }
        public RelayCommand PricingRangeCommand { get; set; }

        /// <summary>
        /// Public constructor
        /// </summary>
        public DemandCurveExponentialViewModel()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            GetXRangeCommand = new RelayCommand(param => GetXRange(), param => true);
            GetYRangeCommand = new RelayCommand(param => GetYRange(), param => true);
            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);

            FigureOutput = new RelayCommand(param => UpdateFigureOutput(), param => true);
            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            ConsumptionRangeCommand = new RelayCommand(param => UpdateConsumptionRange(), param => true);
            PricingRangeCommand = new RelayCommand(param => UpdatePricingRange(), param => true);

            modelArraySelection = "Exponential";
        }

        /// <summary>
        /// Query user for a range
        /// </summary>
        private void UpdatePricingRange()
        {
            var mWin = new RangePrompt();
            mWin.Topmost = true;
            mWin.Owner = windowRef;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (mWin.ShowDialog() == true)
            {
                string[] addresses = mWin.ResponseText.Split(':');

                if (addresses.Length != 2) return;

                var firstChars = new String(addresses[0].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var firstNums = new String(addresses[0].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                var secondChars = new String(addresses[1].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var secondNums = new String(addresses[1].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                int fNum, sNum;

                if (int.TryParse(firstNums, out fNum) && int.TryParse(secondNums, out sNum) && firstChars.Length > 0 && secondChars.Length > 0)
                {
                    if ((sNum - fNum) == 0)
                    {
                        XBrush = Brushes.LightBlue;
                        XRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                        lowColX = DataGridTools.GetColumnIndex(firstChars);
                        highColX = DataGridTools.GetColumnIndex(secondChars);

                        lowRowX = fNum;
                        highRowX = sNum;
                    }
                    else
                    {
                        MessageBox.Show("Please ensure that only a single row is selected");
                    }
                }
                else
                {
                    MessageBox.Show("Parse error!");
                }
            }
        }

        /// <summary>
        /// Query user for a range
        /// </summary>
        private void UpdateConsumptionRange()
        {
            var mWin = new RangePrompt();
            mWin.Topmost = true;
            mWin.Owner = windowRef;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (mWin.ShowDialog() == true)
            {
                string[] addresses = mWin.ResponseText.Split(':');

                if (addresses.Length != 2) return;

                var firstChars = new String(addresses[0].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var firstNums = new String(addresses[0].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                var secondChars = new String(addresses[1].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var secondNums = new String(addresses[1].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                int fNum, sNum;

                if (int.TryParse(firstNums, out fNum) && int.TryParse(secondNums, out sNum) && firstChars.Length > 0 && secondChars.Length > 0)
                {
                    if ((sNum - fNum) == 0)
                    {
                        YBrush = Brushes.LightGreen;
                        YRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                        lowColY = DataGridTools.GetColumnIndex(firstChars);
                        highColY = DataGridTools.GetColumnIndex(secondChars);

                        lowRowY = fNum;
                        highRowY = sNum;
                    }
                    else
                    {
                        MessageBox.Show("Please ensure that only a single row is selected");
                    }
                }
                else
                {
                    MessageBox.Show("Parse error!");
                }
            }
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        private void UpdateFigureOutput()
        {
            outputFigures = !outputFigures;
        }

        /// <summary>
        /// Command-based update of UI logic in VM
        /// </summary>
        private void UpdateSettings()
        {
            if (!AdvancedMenu)
            {
                modelArraySelection = "Exponential";
                AdvancedMenu = !AdvancedMenu;
            }

        }

        /// <summary>
        /// Command-based update of UI logic during close.
        /// Will retain window position in Settings.settings
        /// </summary>
        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Command-based update of UI logic during open.
        /// Will re-check for R interactivity
        /// </summary>
        private void ViewLoaded()
        {
            mWindow.OutputEvents("---------------------------------------------------");
            failed = false;

            try
            {
                REngine.SetEnvironmentVariables();
                engine = REngine.GetInstance();
                engine.Initialize();
                engine.AutoPrint = false;
            }
            catch (Exception e)
            {
                mWindow.OutputEvents(e.ToString());
                failed = true;
            }

            if (failed)
            {
                mWindow.OutputEvents("R components modules were not found!");
                mWindow.OutputEvents("Calculation cannot continue");
                mWindow.OutputEvents("Connect to the internet and re-start the program");
                mWindow.OutputEvents("");
                mWindow.OutputEvents("");

                MessageBox.Show("Modules for R were not found.  Please connect to the internet and restart the program.");
            }
            else
            {
                mWindow.OutputEvents("All R system components modules loaded.");
                mWindow.OutputEvents("Loading Curve Fitting modules and R interface...");
                mWindow.OutputEvents("");
                mWindow.OutputEvents("");
            }

            DefaultFieldsToGray();            

        }

        /// <summary>
        /// Function to update text field background.
        /// Text field background colors as RED indicates the field is actively waiting for select input
        /// </summary>
        private void DefaultFieldsToGray()
        {
            if (YRangeValues.Length < 1 || YRangeValues.ToLower().Contains("spreadsheet"))
            {
                YBrush = Brushes.LightGray;
                YRangeValues = string.Empty;
            }

            if (XRangeValues.Length < 1 || XRangeValues.ToLower().Contains("spreadsheet"))
            {
                XBrush = Brushes.LightGray;
                XRangeValues = string.Empty;
            }
        }

        /// <summary>
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetXRange()
        {
            DefaultFieldsToGray();

            XBrush = Brushes.Yellow;
            XRangeValues = "Select pricing data on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_X;

        }

        /// <summary>
        /// Linq companion for referencing object's location in collection.
        /// </summary>
        /// <param name="model">
        /// Individual row model reference
        /// </param>
        /// <param name="coll">
        /// Collection overall
        /// </param>
        /// <returns>
        /// int-based index
        /// </returns>
        private int GetIndexViewModel(RowViewModel model, ObservableCollection<RowViewModel> coll)
        {
            return coll.IndexOf(model);
        }

        /// <summary>
        /// Delegate after highlighting takes place on datagrid (call back specific to values).
        /// </summary>
        private void DataGrid_PreviewMouseUp_X(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            if (cells.Count < 1) return;

            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;
            lowRowX = cells.Min(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowX = cells.Max(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));

            lowColX = cells.Min(i => i.Column.DisplayIndex);
            highColX = cells.Max(i => i.Column.DisplayIndex);

            if ((highRowX - lowRowX) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_X;

                lowColX = -1;
                lowRowX = -1;
                highColX = -1;
                highRowX = -1;
                MessageBox.Show("Please select a single horizontal row (increasing, from left to right).  You can have many columns, but just one row.");

                return;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_X;

            XBrush = Brushes.LightBlue;
            XRangeValues = DataGridTools.GetColumnName(lowColX) + lowRowX.ToString() + ":" + DataGridTools.GetColumnName(highColX) + highRowX.ToString();
        }

        /// <summary>
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetYRange()
        {
            DefaultFieldsToGray();

            YBrush = Brushes.Yellow;
            YRangeValues = "Select consumption data on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Y;

        }

        /// <summary>
        /// Delegate after highlighting takes place on datagrid (call back specific to values).
        /// </summary>
        private void DataGrid_PreviewMouseUp_Y(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            if (cells.Count < 1) return;

            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;
            lowRowY = cells.Min(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowY = cells.Max(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));

            lowColY = cells.Min(i => i.Column.DisplayIndex);
            highColY = cells.Max(i => i.Column.DisplayIndex);

            if ((highRowY - lowRowY) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

                lowColY = -1;
                lowRowY = -1;
                highColY = -1;
                highRowY = -1;
                MessageBox.Show("Please select a single horizontal row (increasing, from left to right).  You can have many columns, but just one row.");

                return;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

            YBrush = Brushes.LightGreen;
            YRangeValues = DataGridTools.GetColumnName(lowColY) + lowRowY.ToString() + ":" + DataGridTools.GetColumnName(highColY) + highRowY.ToString();
        }

        /// <summary>
        /// Walk through ranged values as needed, finding necessary pairs
        /// </summary>
        /// <param name="startColDelay">
        /// First column index for delay
        /// </param>
        /// <param name="endColDelay">
        /// Final column index for delay
        /// </param>
        /// <param name="rowDelay">
        /// Row index for delays
        /// </param>
        /// <param name="startColValue">
        /// First column index for values
        /// </param>
        /// <param name="endColValue">
        /// Final column index for values
        /// </param>
        /// <param name="rowValue">
        /// Row index for values
        /// </param>
        /// <returns>
        /// List of all range/value pairs that correspond
        /// </returns>
        private List<double>[] GetRangedValues(int startColDelay, int endColDelay, int rowDelay, int startColValue, int endColValue, int rowValue)
        {
            List<double>[] array = new List<double>[2];
            array[0] = new List<double>();
            array[1] = new List<double>();

            string mCellDelay, mCellValue;

            double testDelay = -1,
                   testValue = -1;

            int i = startColDelay,
                j = startColValue;

            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (itemSource == null)
                return null;

            for (; i <= endColDelay && j <= endColValue;)
            {
                mCellDelay = itemSource[rowDelay].values[i];
                mCellValue = itemSource[rowValue].values[j];

                if (Double.TryParse(mCellDelay, out testDelay) && Double.TryParse(mCellValue, out testValue))
                {
                    array[0].Add(testDelay);
                    array[1].Add(testValue);
                }

                i++;
                j++;
            }

            return array;
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {

            if (failed) return;

            List<double>[] array = GetRangedValues(lowColX, highColX, lowRowX, lowColY, highColY, lowRowY);
            List<double> xRange = new List<double>(array[0]);
            List<double> yRange = new List<double>(array[1]);

            bool isExponential = (modelArraySelection == "Exponential");

            if (xRange == null || yRange == null) return;

            // Are zeroes in y range?
            var yQuery = (from y in yRange
                         where y == 0
                         select y).Any();

            List<double> xCopy, yCopy;

            var yTempCheck = new List<double>(yRange);
            var xTempCheck = new List<double>(xRange);
            var pTempCheck = new List<double>();

            foreach (var y in yTempCheck)
            {
                pTempCheck.Add(1);
            }

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            NumericVector yValuesCheck = engine.CreateNumericVector(yTempCheck.ToArray());
            engine.SetSymbol("yLoad", yValuesCheck);

            NumericVector xValuesCheck = engine.CreateNumericVector(xTempCheck.ToArray());
            engine.SetSymbol("xLoad", xValuesCheck);

            NumericVector pValuesCheck = engine.CreateNumericVector(pTempCheck.ToArray());
            engine.SetSymbol("pLoad", pValuesCheck);

            engine.Evaluate(DemandFunctionSolvers.GetSteinSystematicCheck());

            var results = engine.Evaluate("dfres").AsDataFrame();
            var colNames = results.ColumnNames;

            var outputter = colNames[0].ToString().Trim().PadRight(14, ' ') +
                colNames[1].ToString().Trim().PadRight(14, ' ') +
                colNames[2].ToString().Trim().PadRight(14, ' ') +
                colNames[3].ToString().Trim().PadRight(14, ' ') +
                colNames[4].ToString().Trim().PadRight(14, ' ') +
                colNames[5].ToString().Trim().PadRight(14, ' ') +
                colNames[6].ToString().Trim().PadRight(14, ' ') +
                colNames[7].ToString().Trim().PadRight(14, ' ') +
                colNames[8].ToString().Trim().PadRight(14, ' ');

            foreach (var row in results.GetRows())
            {
                outputter = outputter + "\n" + row["Participant"].ToString().Trim().PadRight(14, ' ') +
                    row["TotalPass"].ToString().Trim().PadRight(14, ' ') +
                    row["DeltaQ"].ToString().Trim().PadRight(14, ' ') +
                    row["DeltaQPass"].ToString().Trim().PadRight(14, ' ') +
                    row["Bounce"].ToString().Trim().PadRight(14, ' ') +
                    row["BouncePass"].ToString().Trim().PadRight(14, ' ') +
                    row["Reversals"].ToString().Trim().PadRight(14, ' ') +
                    row["ReversalsPass"].ToString().Trim().PadRight(14, ' ') +
                    row["NumPosValues"].ToString().Trim().PadRight(14, ' ');
            }

            mWindow.OutputEvents(outputter);

            var winHack = new CheckWindow(new string[] { "I'd like to proceed", "I'd like to review my data" }, "I'd like to proceed");
            System.Windows.Documents.Paragraph para = new System.Windows.Documents.Paragraph();
            para.Inlines.Add(outputter);
            winHack.outputWindow.Document.Blocks.Add(para);
            winHack.outputWindow.ScrollToEnd();

            winHack.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            winHack.Title = "Results of Stein Test";
            winHack.Owner = windowRef;
            winHack.Width = 650;
            winHack.Height = 400;
            winHack.Topmost = true;

            if (winHack.ShowDialog() == true)
            {
                if (winHack.MessageOptions.SelectedIndex == 1)
                {
                    return;
                }
            }

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            if (yQuery)
            {
                var yValueWindow = new SelectionWindow(new string[] { "Drop Zeroes", "Change Hundredth", "One Percent of Lowest" }, "Drop Zeroes");
                yValueWindow.Title = "Zero values found in Consumption";
                yValueWindow.MessageLabel.Text = "Please select how to manage the zero values";
                yValueWindow.Owner = windowRef;
                yValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                yValueWindow.Topmost = true;

                if (isExponential)
                {
                    if (yValueWindow.ShowDialog() == true)
                    {
                        int output = yValueWindow.MessageOptions.SelectedIndex;

                        if (output == 0)
                        {
                            xCopy = new List<double>();
                            yCopy = new List<double>();

                            for (int i = 0; i < yRange.Count; i++)
                            {
                                if (yRange[i] == 0)
                                {
                                    // Nothing, not even x added
                                }
                                else
                                {
                                    yCopy.Add(yRange[i]);
                                    xCopy.Add(xRange[i]);
                                }
                            }

                            xRange = new List<double>(xCopy);
                            yRange = new List<double>(yCopy);
                        }
                        else if (output == 1)
                        {
                            yCopy = new List<double>();

                            foreach (double y in yRange)
                            {
                                if (y == 0)
                                {
                                    yCopy.Add(0.01);
                                }
                                else
                                {
                                    yCopy.Add(y);
                                }
                            }

                            yRange = new List<double>(yCopy);
                        }
                        if (output == 2)
                        {
                            double yLow = yRange.Where(y => y > 0).Min(y => y);
                            yLow = yLow / 100;

                            yCopy = new List<double>();

                            foreach (double y in yRange)
                            {
                                if (y == 0)
                                {
                                    yCopy.Add(yLow);
                                }
                                else
                                {
                                    yCopy.Add(y);
                                }
                            }

                            yRange = new List<double>(yCopy);
                        }
                    }
                }              
            }

            // Are zeroes in y range?
            var xQuery = (from x in xRange
                          where x == 0
                          select x).Any();

            if (xQuery)
            {
                var xValueWindow = new SelectionWindow(new string[] { "Change Hundredth", "Drop Zeroes" }, "Change Hundredth");
                xValueWindow.Title = "Zero values found in Pricing";
                xValueWindow.MessageLabel.Text = "Please select how to manage the zero values";
                xValueWindow.Owner = windowRef;
                xValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                xValueWindow.Topmost = true;

                if (xValueWindow.ShowDialog() == true)
                {
                    int output = xValueWindow.MessageOptions.SelectedIndex;

                    List<double> copy;

                    if (output == 0)
                    {
                        copy = new List<double>();

                        foreach (double x in xRange)
                        {
                            if (x == 0)
                            {
                                copy.Add(0.01);
                            }
                            else
                            {
                                copy.Add(x);
                            }
                        }

                        xRange = new List<double>(copy);
                    }
                    else if (output == 1)
                    {
                        xCopy = new List<double>();
                        yCopy = new List<double>();

                        for (int i = 0; i < xRange.Count; i++)
                        {
                            if (xRange[i] == 0)
                            {
                                // Nothing, not even x added
                            }
                            else
                            {
                                yCopy.Add(yRange[i]);
                                xCopy.Add(xRange[i]);
                            }
                        }

                        xRange = new List<double>(xCopy);
                        yRange = new List<double>(yCopy);
                    }
                }
            }

            double lowY = yRange.Where(v => v > 0).OrderBy(v => v).First();
            double highY = yRange.Where(v => v > 0).OrderBy(v => v).Last();

            kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;

            double tempVal;

            if (double.TryParse(KValue, out tempVal))
            {
                var kValueWindow = new SelectionWindow(new string[] { "Use derived K", "Use Custom K" }, "Use derived K");
                kValueWindow.Title = "Multiple K Sources";
                kValueWindow.MessageLabel.Text = "Please select where K should come from:";
                kValueWindow.Owner = windowRef;
                kValueWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                kValueWindow.Topmost = true;

                if (kValueWindow.ShowDialog() == true)
                {
                    int output = kValueWindow.MessageOptions.SelectedIndex;

                    if (output == 0)
                    {
                        kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;
                    }
                    else if (output == 1)
                    {
                        double.TryParse(KValue, out kValueDouble);
                    }
                }
            }

            mWindow.OutputEvents("---------------------------------------------------");

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            NumericVector yValues = engine.CreateNumericVector(yRange.ToArray());
            engine.SetSymbol("yLoad", yValues);

            NumericVector xValues = engine.CreateNumericVector(xRange.ToArray());
            engine.SetSymbol("xLoad", xValues);

            List<double> kRange = new List<double>();
            List<double> pRange = new List<double>();

            for (int i=0; i<xRange.Count; i++)
            {
                kRange.Add(kValueDouble);
                pRange.Add(1);
            }

            NumericVector participantValues = engine.CreateNumericVector(pRange.ToArray());
            engine.SetSymbol("pLoad", participantValues);
            
            NumericVector kValues = engine.CreateNumericVector(kRange.ToArray());
            engine.SetSymbol("kLoad", kValues);

            try
            {
                if (modelArraySelection == "Exponential")
                {
                    engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunction());
                }
                else if (modelArraySelection == "Exponentiated")
                {
                    engine.Evaluate(DemandFunctionSolvers.GetExponentiatedDemandFunction());
                }

                var mWin = new ResultsWindow();
                var mVM = new ResultsViewModel();
                mWin.DataContext = mVM;
                mWin.Owner = windowRef;
                mWin.Width = 500;
                mWin.Height = 500;
                mWin.Topmost = true;

                if(true)
                {
                    for (int i = 0; i < 35; i++)
                    {
                        mVM.RowViewModels.Add(new RowViewModel());
                    }

                    mVM.RowViewModels[0].values[0] = "Results of Fitting";
                    mVM.RowViewModels[1].values[0] = "X Values";
                    mVM.RowViewModels[2].values[0] = "Y Values";

                    for (int i = 0; i < xRange.Count; i++)
                    {
                        mVM.RowViewModels[1].values[1 + i] = xRange[i].ToString();
                        mVM.RowViewModels[2].values[1 + i] = yRange[i].ToString();
                    }

                    mVM.RowViewModels[3].values[0] = "K Value";
                    mVM.RowViewModels[3].values[1] = kValueDouble.ToString();

                    mVM.RowViewModels[5].values[0] = "Parameters";

                    mVM.RowViewModels[6].values[0] = "q0";
                    mVM.RowViewModels[6].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0").AsVector().First().ToString();

                    mVM.RowViewModels[7].values[0] = "alpha";
                    mVM.RowViewModels[7].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alpha").AsVector().First().ToString();

                    mVM.RowViewModels[9].values[0] = "Standard Error";

                    mVM.RowViewModels[10].values[0] = "q0 (se)";
                    mVM.RowViewModels[10].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0err").AsVector().First().ToString();

                    mVM.RowViewModels[11].values[0] = "alpha (se)";
                    mVM.RowViewModels[11].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alphaerr").AsVector().First().ToString();

                    mVM.RowViewModels[13].values[0] = "95% CI";

                    string qLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0low").AsVector().First().ToString();
                    string qHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0high").AsVector().First().ToString();

                    string aLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$alow").AsVector().First().ToString();
                    string aHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$ahigh").AsVector().First().ToString();

                    mVM.RowViewModels[14].values[0] = "Q0";
                    mVM.RowViewModels[14].values[1] = qLow + " - " + qHigh;

                    mVM.RowViewModels[15].values[0] = "alpha";
                    mVM.RowViewModels[15].values[1] = aLow + " - " + aHigh;

                    mVM.RowViewModels[17].values[0] = "Goodness of Fitness";

                    mVM.RowViewModels[18].values[0] = "R-Squared";
                    mVM.RowViewModels[18].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$r2").AsVector().First().ToString();

                    mVM.RowViewModels[19].values[0] = "Abs. Sum Squares";
                    mVM.RowViewModels[19].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$absSS").AsVector().First().ToString();

                    mVM.RowViewModels[20].values[0] = "Resid. SD";
                    mVM.RowViewModels[20].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$sdResid").AsVector().First().ToString();

                    mWin.Show();
                }

                if (outputFigures)
                {
                    try
                    {
                        if (modelArraySelection == "Exponential")
                        {
                            engine.Evaluate(DemandFunctionSolvers.GetExponentialGraphingFunction());
                        }
                        else if (modelArraySelection == "Exponentiated")
                        {
                            engine.Evaluate(DemandFunctionSolvers.GetExponentiatedGraphingFunction());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
