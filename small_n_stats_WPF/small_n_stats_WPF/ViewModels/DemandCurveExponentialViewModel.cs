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
        /// Delegate after highlighting takes place on datagrid (call back specific to values).
        /// </summary>
        private void DataGrid_PreviewMouseUp_X(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();
            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (cells.Count < 1 || itemSource.Count < 1) return;

            lowRowX = cells.Min(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowX = cells.Max(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));

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
            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (cells.Count < 1 || itemSource.Count < 1) return;

            lowRowY = cells.Min(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowY = cells.Max(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));

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
            mWindow.dataGrid.CommitEdit();

            if (failed) return;

            double derivedK = -1;

            mWindow.OutputEvents(" ");
            mWindow.OutputEvents("---------------------------------------------------");

            mWindow.OutputEvents("Checking user-supplied ranges and reference points....");

            List<double>[] array = GetRangedValues(lowColX, highColX, lowRowX, lowColY, highColY, lowRowY);
            List<double> xRange = new List<double>(array[0]);
            List<double> yRange = new List<double>(array[1]);

            double lowY = yRange.Where(v => v > 0).OrderBy(v => v).First();
            double highY = yRange.Where(v => v > 0).OrderBy(v => v).Last();

            kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;

            mWindow.OutputEvents("Data passed null and type checks...");
            mWindow.OutputEvents("Determining a fitting heuristic...");

            #region FittingHeuristic

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            YValueDecisions yBehavior = Decisions.GetYBehavior(modelArraySelection, windowRef);
            XValueDecisions xBehavior = Decisions.GetXBehavior(modelArraySelection, windowRef);
            KValueDecisions kBehavior = Decisions.GetKBehaviorIndividual(windowRef);

            if (kBehavior == KValueDecisions.DeriveValuesIndividual)
            {
                kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;
            }
            else if (kBehavior == KValueDecisions.UseSuppliedValues)
            {
                if (!double.TryParse(KValue, out kValueDouble))
                {
                    mWindow.OutputEvents("Your supplied K value does not appear correct.");
                    MessageBox.Show("Your supplied K value does not appear correct.");
                    return;
                }
            }

            mWindow.OutputEvents("---------------------------------------------------");

            List<double> xTemp = new List<double>();
            List<double> yTemp = new List<double>();
            List<double> pTemp = new List<double>();

            for (int j = 0; j < xRange.Count; j++)
            {
                yTemp.Add(yRange[j]);
                xTemp.Add(xRange[j]);
                pTemp.Add(1);
            }

            /* Have total data here, Commence search for mods */

            for (int i = 0; i < xTemp.Count; i++)
            {
                if (xBehavior == XValueDecisions.ChangeHundredth && xTemp[i] == 0)
                {
                    xTemp[i] = 0.01;
                }
            }

            for (int i = 0; i < yTemp.Count; i++)
            {
                if (yBehavior == YValueDecisions.ChangeHundredth && yTemp[0] == 0)
                {
                    yTemp[i] = 0.01;
                }
                else if (yBehavior == YValueDecisions.OnePercentLowest && yTemp[0] == 0)
                {
                    yTemp[i] = lowY / 100;
                }
            }

            List<int> indicesToRemove = new List<int>();

            for (int i = 0; i < xTemp.Count; i++)
            {
                if (xBehavior == XValueDecisions.DropZeros && xTemp[i] == 0)
                {
                    indicesToRemove.Add(i);
                }
                else if (yBehavior == YValueDecisions.DropZeros && yTemp[i] == 0)
                {
                    indicesToRemove.Add(i);
                }
            }

            if (indicesToRemove.Count > 0)
            {
                indicesToRemove.Sort();
                indicesToRemove.Reverse();

                foreach (int index in indicesToRemove)
                {
                    yTemp.RemoveAt(index);
                    xTemp.RemoveAt(index);
                    pTemp.RemoveAt(index);
                }
            }

            #endregion

            #region SteinTest

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            NumericVector yValuesCheck = engine.CreateNumericVector(yTemp.ToArray());
            engine.SetSymbol("yLoad", yValuesCheck);

            NumericVector xValuesCheck = engine.CreateNumericVector(xTemp.ToArray());
            engine.SetSymbol("xLoad", xValuesCheck);

            NumericVector pValuesCheck = engine.CreateNumericVector(pTemp.ToArray());
            engine.SetSymbol("pLoad", pValuesCheck);

            engine.Evaluate(DemandFunctionSolvers.GetSteinSystematicCheck());

            var results = engine.Evaluate("SteinFrame").AsDataFrame();
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

            #endregion

            #region ValueSets

            mWindow.OutputEvents("All inputs passed verification.");
            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Beginning Batched Computations...");

            var mWin = new ResultsWindow();
            var mVM = new ResultsViewModel();
            mWin.DataContext = mVM;

            for (int i = 0; i < yRange.Count + 5; i++)
            {
                mVM.RowViewModels.Add(new RowViewModel());
            }

            derivedK = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;

            #endregion

            #region IndividualFittings

            try
            {
                List<double> kRange = new List<double>();
                List<double> pRange = new List<double>();

                mVM.RowViewModels[0].values[0] = "Results of Fitting";
                mVM.RowViewModels[0].values[1] = "K Value";
                mVM.RowViewModels[0].values[2] = "q0";
                mVM.RowViewModels[0].values[3] = "alpha";

                mVM.RowViewModels[0].values[4] = "q0 (se)";
                mVM.RowViewModels[0].values[5] = "alpha (se)";
                mVM.RowViewModels[0].values[6] = "Q0 (95% CI)";
                mVM.RowViewModels[0].values[7] = "alpha (95% CI)";

                mVM.RowViewModels[0].values[8] = "R-Squared";
                mVM.RowViewModels[0].values[9] = "Abs. Sum Squares";
                mVM.RowViewModels[0].values[10] = "Resid. SD";

                mVM.RowViewModels[0].values[11] = "Empirical oMax";
                mVM.RowViewModels[0].values[12] = "Empirical pMax";
                mVM.RowViewModels[0].values[13] = "Q0e";
                mVM.RowViewModels[0].values[14] = "BP0";
                mVM.RowViewModels[0].values[15] = "BP1";
                mVM.RowViewModels[0].values[16] = "EV";
                mVM.RowViewModels[0].values[17] = "Derived oMax";
                mVM.RowViewModels[0].values[18] = "Derived pMax";

                mVM.RowViewModels[0].values[19] = "Pricing";
                mVM.RowViewModels[0].values[20] = "Consumption";
                mVM.RowViewModels[0].values[21] = "Notes";

                mVM.RowViewModels[1].values[0] = "Series #" + (1).ToString();

                NumericVector yValues = null;
                NumericVector xValues = null;

                if (yBehavior == YValueDecisions.DoNothing)
                {
                    // Nothing different
                }
                else if (yBehavior == YValueDecisions.ChangeHundredth)
                {
                    List<double> yCopy = new List<double>();

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
                else if (yBehavior == YValueDecisions.OnePercentLowest)
                {
                    double yLow = yRange.Where(y => y > 0).Min(y => y);
                    yLow = yLow / 100;

                    List<double> yCopy = new List<double>();

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
                else if (yBehavior == YValueDecisions.DropZeros)
                {
                    List<int> removeList = new List<int>();

                    for (int i = 0; i < yRange.Count(); i++)
                    {
                        if (yRange[i] == 0)
                        {
                            removeList.Add(i);
                        }
                    }

                    List<double> tempX = new List<double>(xRange);
                    List<double> tempY = new List<double>(yRange);

                    if (removeList.Count() > 0)
                    {
                        foreach (int index in removeList)
                        {
                            xRange.Remove(tempX[index]);
                            yRange.Remove(tempY[index]);
                        }
                    }
                }

                if (xBehavior == XValueDecisions.DoNothing)
                {
                    // Do nothing different
                }
                else if (xBehavior == XValueDecisions.ChangeHundredth)
                {
                    for (int i = 0; i < xRange.Count(); i++)
                    {
                        if (xRange[i] == 0.0)
                        {
                            xRange[i] = 0.01;
                        }
                    }
                }
                else if (xBehavior == XValueDecisions.DropZeros)
                {
                    List<int> removeList = new List<int>();

                    for (int i = 0; i < xRange.Count(); i++)
                    {
                        if (xRange[i] == 0)
                        {
                            removeList.Add(i);
                        }
                    }

                    List<double> tempX = new List<double>(xRange);
                    List<double> tempY = new List<double>(yRange);

                    if (removeList.Count() > 0)
                    {
                        foreach (int index in removeList)
                        {
                            xRange.Remove(tempX[index]);
                            yRange.Remove(tempY[index]);
                        }
                    }
                }

                if (yRange.Count() < 3)
                {
                    for (int i = 2; i <= 10; i++)
                    {
                        mVM.RowViewModels[1].values[i] = "NA";
                    }

                    mVM.RowViewModels[1].values[11] = string.Join(",", xRange);
                    mVM.RowViewModels[1].values[12] = string.Join(",", yRange);
                    mVM.RowViewModels[1].values[13] = "Model could not be run, fewer than 3 data points were present?";
                }

                for (int i = 0; i < xRange.Count; i++)
                {
                    if (kBehavior == KValueDecisions.DeriveValuesGroup)
                    {
                        kRange.Add(derivedK);
                    }
                    else if (kBehavior == KValueDecisions.DeriveValuesIndividual)
                    {
                        double hi = yRange.Where(v => v > 0).ToList().Max();
                        double lo = yRange.Where(v => v > 0).ToList().Min();
                        double indivK = (Math.Log10(hi) - Math.Log10(lo)) + 0.5;

                        kRange.Add(indivK);
                    }
                    else if (kBehavior == KValueDecisions.UseSuppliedValues)
                    {
                        kRange.Add(kValueDouble);
                    }

                    pRange.Add(1);
                }

                NumericVector kValues = engine.CreateNumericVector(kRange.ToArray());
                engine.SetSymbol("kLoad", kValues);

                NumericVector participantValues = engine.CreateNumericVector(pRange.ToArray());
                engine.SetSymbol("pLoad", participantValues);

                if (modelArraySelection == "Exponential")
                {
                    yValues = engine.CreateNumericVector(yRange.ToArray());
                    engine.SetSymbol("yLoad", yValues);

                    xValues = engine.CreateNumericVector(xRange.ToArray());
                    engine.SetSymbol("xLoad", xValues);

                    if (kBehavior == KValueDecisions.FitK)
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunctionKFittings());
                    }
                    else
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunction());
                    }

                }
                else if (modelArraySelection == "Exponentiated")
                {
                    yValues = engine.CreateNumericVector(yRange.ToArray());
                    engine.SetSymbol("yLoad", yValues);

                    xValues = engine.CreateNumericVector(xRange.ToArray());
                    engine.SetSymbol("xLoad", xValues);

                    if (kBehavior == KValueDecisions.FitK)
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentiatedDemandFunctionKFittings());
                    }
                    else
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentiatedDemandFunction());
                    }
                }

                // NA's default to true in R.Net
                if (engine.Evaluate("fitFrame[fitFrame$p==1,]$q0").AsVector().First().ToString() != "True")
                {
                    if (kBehavior == KValueDecisions.FitK)
                    {
                        mVM.RowViewModels[1].values[1] = engine.Evaluate("fitFrame[fitFrame$p==1,]$k").AsVector().First().ToString();
                    }
                    else
                    {
                        mVM.RowViewModels[1].values[1] = kRange.Min().ToString();
                    }

                    mVM.RowViewModels[1].values[2] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[3] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alpha").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[4] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0err").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[5] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alphaerr").AsVector().First().ToString();

                    string qLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0low").AsVector().First().ToString();
                    string qHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0high").AsVector().First().ToString();
                    string aLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$alow").AsVector().First().ToString();
                    string aHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$ahigh").AsVector().First().ToString();

                    mVM.RowViewModels[1].values[6] = qLow + " - " + qHigh;
                    mVM.RowViewModels[1].values[7] = aLow + " - " + aHigh;

                    mVM.RowViewModels[1].values[8] = engine.Evaluate("fitFrame[fitFrame$p==1,]$r2").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[9] = engine.Evaluate("fitFrame[fitFrame$p==1,]$absSS").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[10] = engine.Evaluate("fitFrame[fitFrame$p==1,]$sdResid").AsVector().First().ToString();

                    mVM.RowViewModels[1].values[11] = engine.Evaluate("fitFrame[fitFrame$p==1,]$OmaxE").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[12] = engine.Evaluate("fitFrame[fitFrame$p==1,]$PmaxE").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[13] = engine.Evaluate("fitFrame[fitFrame$p==1,]$Q0e").AsVector().First().ToString();

                    if (engine.Evaluate("fitFrame[fitFrame$p==1,]$BP0").AsVector().First().ToString() == "True")
                    {
                        mVM.RowViewModels[1].values[14] = "NA";
                    }
                    else
                    {
                        mVM.RowViewModels[1].values[14] = engine.Evaluate("fitFrame[fitFrame$p==1,]$BP0").AsVector().First().ToString();
                    }

                    mVM.RowViewModels[1].values[15] = engine.Evaluate("fitFrame[fitFrame$p==1,]$BP1").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[16] = engine.Evaluate("fitFrame[fitFrame$p==1,]$EV").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[17] = engine.Evaluate("fitFrame[fitFrame$p==1,]$OmaxD").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[18] = engine.Evaluate("fitFrame[fitFrame$p==1,]$PmaxD").AsVector().First().ToString();

                    mVM.RowViewModels[1].values[19] = string.Join(",", xValues);
                    mVM.RowViewModels[1].values[20] = string.Join(",", yValues);
                }
                else
                {
                    for (int i = 2; i <= 18; i++)
                    {
                        mVM.RowViewModels[1].values[i] = "NA";
                    }

                    mVM.RowViewModels[1].values[19] = string.Join(",", xValues);
                    mVM.RowViewModels[1].values[20] = string.Join(",", yValues);
                    mVM.RowViewModels[1].values[21] = "Model did not converge, was a curve actually present?";
                }

                mWin.Show();

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
            catch (ParseException pe)
            {
                Console.WriteLine(pe.ToString());
            }

            #endregion
            
        }
    }
}
