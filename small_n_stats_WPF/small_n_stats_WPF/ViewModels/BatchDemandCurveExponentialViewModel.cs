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
    class BatchDemandCurveExponentialViewModel : BaseViewModel
    {
        public MainWindow mWindow { get; set; }
        public MainWindowViewModel mViewModel { get; set; }
        public BatchDemandCurveWindow windowRef { get; set; }
        
        private bool runExponential = false;
        public bool RunExponential
        {
            get { return runExponential; }
            set
            {
                runExponential = value;
                OnPropertyChanged("RunExponential");
            }
        }

        private bool runExponentiated = false;
        public bool RunExponentiated
        {
            get { return runExponentiated; }
            set
            {
                runExponentiated = value;
                OnPropertyChanged("RunExponentiated");
            }
        }

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

        private string kRangeValues = "";
        public string KRangeValues
        {
            get { return kRangeValues; }
            set
            {
                kRangeValues = value;
                OnPropertyChanged("KRangeValues");
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

        private double kValueDouble = 0;

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

        private string selectedMode = "Individual";
        public string SelectedMode
        {
            get { return selectedMode; }
            set
            {
                selectedMode = value;
                OnPropertyChanged("SelectedMode");
            }
        }

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

        private Brush kBrush = Brushes.White;
        public Brush KBrush
        {
            get { return kBrush; }
            set
            {
                kBrush = value;
                OnPropertyChanged("KBrush");
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

        bool failed;

        int lowRowX = 0,
            highRowX = 0,
            lowColX = 0,
            highColX = 0;

        int lowRowY = 0,
            highRowY = 0,
            lowColY = 0,
            highColY = 0;

        int lowRowK = 0,
            highRowK = 0,
            lowColK = 0,
            highColK = 0;

        /* Commands */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand GetXRangeCommand { get; set; }
        public RelayCommand GetYRangeCommand { get; set; }
        public RelayCommand GetKRangeCommand { get; set; }

        public RelayCommand CalculateScoresCommand { get; set; }
        public RelayCommand AdvancedSettings { get; set; }

        public RelayCommand ConsumptionRangeCommand { get; set; }
        public RelayCommand PricingRangeCommand { get; set; }
        public RelayCommand ConstantRangeCommand { get; set; }

        /// <summary>
        /// Public constructor
        /// </summary>
        public BatchDemandCurveExponentialViewModel()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            GetXRangeCommand = new RelayCommand(param => GetXRange(), param => true);
            GetYRangeCommand = new RelayCommand(param => GetYRange(), param => true);
            GetKRangeCommand = new RelayCommand(param => GetKRange(), param => true);

            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);
            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            ConsumptionRangeCommand = new RelayCommand(param => UpdateConsumptionRange(), param => true);
            PricingRangeCommand = new RelayCommand(param => UpdatePricingRange(), param => true);
            ConstantRangeCommand = new RelayCommand(param => UpdateKRange(), param => true);

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
        private void UpdateKRange()
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
                    //Single Column
                    if ((DataGridTools.GetColumnIndex(firstChars) - DataGridTools.GetColumnIndex(secondChars)) == 0)
                    {
                        KBrush = Brushes.LightSalmon;
                        KRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                        lowColK = DataGridTools.GetColumnIndex(firstChars);
                        highColK = DataGridTools.GetColumnIndex(secondChars);

                        lowRowK = fNum;
                        highRowK = sNum;
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
                    if ((sNum - fNum) > 2)
                    {
                        YBrush = Brushes.LightBlue;
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

            if (KRangeValues.Length < 1 || KRangeValues.ToLower().Contains("spreadsheet"))
            {
                KBrush = Brushes.LightGray;
                KRangeValues = string.Empty;
            }
        }

        /// <summary>
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetKRange()
        {
            DefaultFieldsToGray();

            KBrush = Brushes.Yellow;
            KRangeValues = "Select k values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_K;
        }

        /// <summary>
        /// Delegate after highlighting takes place on datagrid (call back specific to values).
        /// </summary>
        private void DataGrid_PreviewMouseUp_K(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;
            lowRowK = cells.Min(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowK = cells.Max(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));

            lowColK = cells.Min(i => i.Column.DisplayIndex);
            highColK = cells.Max(i => i.Column.DisplayIndex);

            if ((highColK - lowColK) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_K;

                lowColK = -1;
                lowRowK = -1;
                highColK = -1;
                highRowK = -1;
                MessageBox.Show("Please select a single vertical column.  You can have many rows, but just one column of K values.");

                return;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_K;

            KBrush = Brushes.LightSalmon;
            KRangeValues = DataGridTools.GetColumnName(lowColK) + lowRowK.ToString() + ":" + DataGridTools.GetColumnName(highColK) + highRowK.ToString();

            KValue = "";
        }

        /// <summary>
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetXRange()
        {
            DefaultFieldsToGray();

            XBrush = Brushes.Yellow;
            XRangeValues = "Select delays on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_X;

        }

        /// <summary>
        /// Delegate after highlighting takes place on datagrid (call back specific to values).
        /// </summary>
        private void DataGrid_PreviewMouseUp_X(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

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
                MessageBox.Show("Please select a single horizontal row.  You can have many columns, but just one row of pricing values.");

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
            YRangeValues = "Select values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Y;
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
        /// Delegate after highlighting takes place on datagrid (call back specific to delays).
        /// </summary>
        private void DataGrid_PreviewMouseUp_Y(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;
            lowRowY = cells.Min(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowY = cells.Max(i => GetIndexViewModel((RowViewModel)i.Item, itemSource));

            lowColY = cells.Min(i => i.Column.DisplayIndex);
            highColY = cells.Max(i => i.Column.DisplayIndex);

            if ((highColY - lowColY) < 2 && (highRowY - lowRowY) < 2)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

                lowColY = -1;
                lowRowY = -1;
                highColY = -1;
                highRowY = -1;
                MessageBox.Show("Please select a matrix of consumption values, with at least three rows of values with three individual points of data (i.e., 3x3).");

                return;
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

            YBrush = Brushes.LightGreen;
            YRangeValues = DataGridTools.GetColumnName(lowColY) + lowRowY.ToString() + ":" + DataGridTools.GetColumnName(highColY) + highRowY.ToString();
        }

        /// <summary>
        /// Function for parsing values of individual cells by referencing view model
        /// </summary>
        private List<double> GetRangedValuesVM(int startCol, int endCol, int startRow)
        {
            List<double> mRange = new List<double>();

            double test;

            for (int i = startCol; i <= endCol; i++)
            {
                string mRowItem = mViewModel.RowViewModels[startRow].values[i];

                if (!Double.TryParse(mRowItem, out test))
                {
                    return null;
                }
                else
                {
                    mRange.Add(test);
                }
            }

            return mRange;
        }

        /// <summary>
        /// Function for parsing values of individual cells by referencing view model
        /// </summary>
        private List<double> GetRangedValuesVerticalVM(int startRow, int endRow, int col)
        {
            List<double> mRange = new List<double>();

            if (startRow == -1 && endRow == -1)
            {
                return null;
            }

            double test;

            for (int i = startRow; i <= endRow; i++)
            {
                string mRowItemCell = mViewModel.RowViewModels[i].values[col];

                if (!Double.TryParse(mRowItemCell, out test))
                {
                    return null;
                }
                else
                {
                    mRange.Add(test);
                }
            }

            return mRange;
        }

        /// <summary>
        /// A method for submitting a string-encoded range and returning the value of the cells selected.
        /// </summary>
        /// <param name="range">
        /// List of double values returned for use as delay or value points in Computation
        /// </param>
        public string[,] ParseBulkRangeStringsVM(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            string[,] mDouble = null;

            double tempHolder;
            List<double> tempHolderList = new List<double>();

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new string[mCols, mRows];

            try
            {

                for (int i = lowRowValue; i <= highRowValue; i++)
                {
                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        string mRowItem = mViewModel.RowViewModels[i].values[j];
                        mDouble[j - lowColValue, i - lowRowValue] = mRowItem;

                        if (double.TryParse(mRowItem, out tempHolder))
                        {
                            tempHolderList.Add(tempHolder);
                        }
                    }
                }
            }
            catch 
            {
                return null;
            }

            var yList = tempHolderList.ToList().Where(e => e >= 1).OrderBy(e => e);
            double kHigh = yList.Max(),
                   kLow = yList.Min();

            kValueDouble = Math.Log10(kHigh) - Math.Log10(kLow);

            return mDouble;
        }

        /// <summary>
        /// Bool check if there are zeroes in the supplied two dimensional arrow
        /// </summary>
        /// <param name="source">
        /// Two dimensional array
        /// </param>
        /// <returns>
        /// Return true if a zero is somewhere in matrix
        /// </returns>
        private bool AreZerosInMatrix(string[,] source)
        {
            int cols = source.GetLength(0);
            int rows = source.GetLength(1);
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int temp;
                    if (int.TryParse(source[j,i], out temp))
                    {
                        if (temp == 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Bool check if there are zeroes in the supplied two dimensional arrow
        /// </summary>
        /// <param name="source">
        /// Two dimensional array
        /// </param>
        /// <returns>
        /// Return two index array of the lowest (1) and highest (2) non-zero elements
        /// </returns>
        private double[] GetLowestAndHighestInMatrix(string[,] source)
        {
            int cols = source.GetLength(0);
            int rows = source.GetLength(1);
            
            double low = 9999999.0;
            double high = 0.0;
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double temp;
                    if (double.TryParse(source[j,i], out temp))
                    {
                        if (temp > high)
                        {
                            high = temp;
                        }
                        else if (temp < low && temp > 0)
                        {
                            low = temp;
                        }
                    }
                }
            }

            return new double[] { low, high };
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
        private YValueDecisions GetYBehavior(string modelType)
        {
            string recommended = (modelType == "Exponential") ? "Drop Zeroes" : "Do Nothing";

            var yValueWindow = new SelectionWindow(new string[] { "Drop Zeroes", "Change Hundredth", "One Percent of Lowest", "Do Nothing" }, recommended);
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
                    return YValueDecisions.OnePercentLowest;
                }
                else if (output == 3)
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
        private XValueDecisions GetXBehavior(string modelType)
        {
            string recommended = (modelType == "Exponential") ? "Drop Zeroes" : "Change Hundredth";

            var xValueWindow = new SelectionWindow(new string[] { "Change Hundredth", "Drop Zeroes", "Do Nothing" }, "Change Hundredth");
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
                    return XValueDecisions.ChangeHundredth;
                }
                else if (output == 1)
                {
                    return XValueDecisions.DropZeros;
                }
                else if (output == 2)
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
        private KValueDecisions GetKBehaviorIndividual()
        {
            var kValueWindow = new SelectionWindow(new string[] { "Use derived K (group)", "Use derived K (individual)", "Use Custom Ks" }, "Use derived K (group)");
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
                    return KValueDecisions.DeriveValuesGroup;
                }
                else if (output == 1)
                {
                    return KValueDecisions.DeriveValuesIndividual;
                }
                else if (output == 2)
                {
                    return KValueDecisions.UseSuppliedValues;
                }
            }

            return KValueDecisions.DeriveValuesGroup;
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
        private KValueDecisions GetKBehaviorGroup()
        {
            var kValueWindow = new SelectionWindow(new string[] { "Fit K as parameter", "Use derived K (group)", "Use Custom Ks" }, "Fit K as parameter");
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
                    return KValueDecisions.UseSuppliedValues;
                }
            }

            return KValueDecisions.FitK;
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {
            if (failed) return;

            bool customK = false;

            double derivedK = -1;

            mWindow.OutputEvents("---------------------------------------------------");
            
            mWindow.OutputEvents("Checking user-supplied ranges and reference points....");
            
            List<double> xRange = GetRangedValuesVM(lowColX, highColX, lowRowX);
            string[,] wholeRange = ParseBulkRangeStringsVM(lowRowY, highRowY, lowColY, highColY);

            if (xRange == null)
            {
                mWindow.OutputEvents("Error while validating the Delays.  There cannot be any blank, null or non-numeric fields.");
                MessageBox.Show("Please review the the Delays column.  There cannot be any blank, null or non-numeric fields.");
                return;
            }

            if (wholeRange == null)
            {
                mWindow.OutputEvents("There were items that failed validation in the Indifference Point values.  Are any fields blank or not numeric?");
                MessageBox.Show("There were items that failed validation in the Indifference Point values.");
                return;
            }

            #region FittingHeuristic

            mWindow.OutputEvents("Data passed null and type checks...");

            mWindow.OutputEvents("Determining a fitting heuristic...");

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            YValueDecisions yBehavior = GetYBehavior(modelArraySelection);
            XValueDecisions xBehavior = GetXBehavior(modelArraySelection);
            KValueDecisions kBehavior = (SelectedMode == "Individual") ? GetKBehaviorIndividual() : GetKBehaviorGroup();

            mWindow.OutputEvents("---------------------------------------------------");

            List<double> xTemp = new List<double>();
            List<double> yTemp = new List<double>();
            List<double> pTemp = new List<double>();

            for (int i = 0; i < wholeRange.GetLength(1); i++)
            {
                for (int j = 0; j < wholeRange.GetLength(0); j++)
                {
                    yTemp.Add(double.Parse(wholeRange[j, i]));
                    xTemp.Add(xRange[j]);
                    pTemp.Add(i + 1);
                }
            }

            /* Have total data here, Commence search for mods */

            double[] lowestAndHighest = GetLowestAndHighestInMatrix(wholeRange);

            for (int i=0; i<xTemp.Count; i++)
            {
                if (xBehavior == XValueDecisions.ChangeHundredth && xTemp[i] == 0)
                {
                    xTemp[i] = 0.01;
                }
            }

            for (int i=0; i<yTemp.Count; i++)
            {
                if (yBehavior == YValueDecisions.ChangeHundredth && yTemp[0] == 0)
                {
                    yTemp[i] = 0.01;
                }
                else if (yBehavior == YValueDecisions.OnePercentLowest && yTemp[0] == 0)
                {
                    yTemp[i] = lowestAndHighest[0]/100;
                }
            }

            List<int> indicesToRemove = new List<int>();

            for (int i = 0; i<xTemp.Count; i++)
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

            List<double> xRangeShadow = new List<double>();
            double holder;

            List<double> yRange = new List<double>();
            xRangeShadow.Clear();

            // Reference point to compare user-supplied k Range with
            for (int i = 0; i < wholeRange.GetLength(0); i++)
            {
                if (double.TryParse(wholeRange[i, 0], out holder))
                {
                    yRange.Add(holder);
                    xRangeShadow.Add(xRange[i]);
                }
            }

            List<double> kRanges = null;

            if (AdvancedMenu)
            {
                kRanges = GetRangedValuesVerticalVM(lowRowK, highRowK, lowColK);

                if (kRanges != null)
                {
                    if (kRanges.Count() > 1 && kRanges.Count() == wholeRange.GetLength(1))
                    {
                        kBehavior = KValueDecisions.UseSuppliedValues;
                        customK = true;
                    }
                    else if (kRanges.Count() > 1 && kRanges.Count() != wholeRange.GetLength(1))
                    {
                        mWindow.OutputEvents("Your custom k ranges don't match the # of rows.");
                        MessageBox.Show("Hmm, check your k range.  It doesn't seem paired up with the rows.");
                        return;
                    }
                }
            }

            mWindow.OutputEvents("All inputs passed verification.");
            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Beginning Batched Computations...");

            var mWin = new ResultsWindow();
            var mVM = new ResultsViewModel();
            mWin.DataContext = mVM;

            for (int i = 0; i < wholeRange.GetLength(1) + 5; i++)
            {
                mVM.RowViewModels.Add(new RowViewModel());
            }

            double[] yLowHigh = GetLowestAndHighestInMatrix(wholeRange);

            derivedK = (Math.Log10(yLowHigh[1]) - Math.Log10(yLowHigh[0])) + 0.5;

            #endregion

            if (SelectedMode == "Individual")
            {

                #region IndividualFittings

                for (int mIndex = 0; mIndex < wholeRange.GetLength(1); mIndex++)
                {
                    engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                    yRange.Clear();
                    xRangeShadow.Clear();

                    for (int i = 0; i < wholeRange.GetLength(0); i++)
                    {
                        if (double.TryParse(wholeRange[i, mIndex], out holder))
                        {
                            yRange.Add(holder);
                            xRangeShadow.Add(xRange[i]);
                        }
                    }

                    try
                    {
                        List<double> kRange = new List<double>();
                        List<double> pRange = new List<double>();

                        if (mIndex == 0)
                        {
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

                        }

                        mVM.RowViewModels[1 + mIndex].values[0] = "Series #" + (mIndex + 1).ToString();

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

                            List<double> tempX = new List<double>(xRangeShadow);
                            List<double> tempY = new List<double>(yRange);

                            if (removeList.Count() > 0)
                            {
                                foreach (int index in removeList)
                                {
                                    xRangeShadow.Remove(tempX[index]);
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
                            for (int i = 0; i < xRangeShadow.Count(); i++)
                            {
                                if (xRangeShadow[i] == 0.0)
                                {
                                    xRangeShadow[i] = 0.01;
                                }
                            }
                        }
                        else if (xBehavior == XValueDecisions.DropZeros)
                        {
                            List<int> removeList = new List<int>();

                            for (int i = 0; i < xRangeShadow.Count(); i++)
                            {
                                if (xRangeShadow[i] == 0)
                                {
                                    removeList.Add(i);
                                }
                            }

                            List<double> tempX = new List<double>(xRangeShadow);
                            List<double> tempY = new List<double>(yRange);

                            if (removeList.Count() > 0)
                            {
                                foreach (int index in removeList)
                                {
                                    xRangeShadow.Remove(tempX[index]);
                                    yRange.Remove(tempY[index]);
                                }
                            }
                        }

                        if (yRange.Count() < 3)
                        {
                            for (int i = 2; i <= 10; i++)
                            {
                                mVM.RowViewModels[1 + mIndex].values[i] = "NA";
                            }

                            mVM.RowViewModels[1 + mIndex].values[11] = string.Join(",", xRangeShadow);
                            mVM.RowViewModels[1 + mIndex].values[12] = string.Join(",", yRange);
                            mVM.RowViewModels[1 + mIndex].values[13] = "Model could not be run, fewer than 3 data points were present?";

                            continue;
                        }

                        for (int i = 0; i < xRangeShadow.Count; i++)
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
                                kRange.Add(kRanges[mIndex]);
                            }

                            pRange.Add(1);
                        }

                        mVM.RowViewModels[1 + mIndex].values[1] = kRange.Min().ToString();

                        NumericVector kValues = engine.CreateNumericVector(kRange.ToArray());
                        engine.SetSymbol("kLoad", kValues);

                        NumericVector participantValues = engine.CreateNumericVector(pRange.ToArray());
                        engine.SetSymbol("pLoad", participantValues);

                        if (modelArraySelection == "Exponential")
                        {

                            yValues = engine.CreateNumericVector(yRange.ToArray());
                            engine.SetSymbol("yLoad", yValues);

                            xValues = engine.CreateNumericVector(xRangeShadow.ToArray());
                            engine.SetSymbol("xLoad", xValues);

                            engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunction());

                        }
                        else if (modelArraySelection == "Exponentiated")
                        {
                            yValues = engine.CreateNumericVector(yRange.ToArray());
                            engine.SetSymbol("yLoad", yValues);

                            xValues = engine.CreateNumericVector(xRangeShadow.ToArray());
                            engine.SetSymbol("xLoad", xValues);

                            engine.Evaluate(DemandFunctionSolvers.GetExponentiatedDemandFunction());
                        }

                        // NA's default to true in R.Net
                        if (engine.Evaluate("fitFrame[fitFrame$p==1,]$q0").AsVector().First().ToString() != "True")
                        {
                            mVM.RowViewModels[1 + mIndex].values[2] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[3] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alpha").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[4] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0err").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[5] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alphaerr").AsVector().First().ToString();

                            string qLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0low").AsVector().First().ToString();
                            string qHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0high").AsVector().First().ToString();
                            string aLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$alow").AsVector().First().ToString();
                            string aHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$ahigh").AsVector().First().ToString();

                            mVM.RowViewModels[1 + mIndex].values[6] = qLow + " - " + qHigh;
                            mVM.RowViewModels[1 + mIndex].values[7] = aLow + " - " + aHigh;

                            mVM.RowViewModels[1 + mIndex].values[8] = engine.Evaluate("fitFrame[fitFrame$p==1,]$r2").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[9] = engine.Evaluate("fitFrame[fitFrame$p==1,]$absSS").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[10] = engine.Evaluate("fitFrame[fitFrame$p==1,]$sdResid").AsVector().First().ToString();

                            mVM.RowViewModels[1 + mIndex].values[11] = engine.Evaluate("fitFrame[fitFrame$p==1,]$OmaxE").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[12] = engine.Evaluate("fitFrame[fitFrame$p==1,]$PmaxE").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[13] = engine.Evaluate("fitFrame[fitFrame$p==1,]$Q0e").AsVector().First().ToString();

                            if (engine.Evaluate("fitFrame[fitFrame$p==1,]$BP0").AsVector().First().ToString() == "True")
                            {
                                mVM.RowViewModels[1 + mIndex].values[14] = "NA";
                            }
                            else
                            {
                                mVM.RowViewModels[1 + mIndex].values[14] = engine.Evaluate("fitFrame[fitFrame$p==1,]$BP0").AsVector().First().ToString();
                            }

                            mVM.RowViewModels[1 + mIndex].values[15] = engine.Evaluate("fitFrame[fitFrame$p==1,]$BP1").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[16] = engine.Evaluate("fitFrame[fitFrame$p==1,]$EV").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[17] = engine.Evaluate("fitFrame[fitFrame$p==1,]$OmaxD").AsVector().First().ToString();
                            mVM.RowViewModels[1 + mIndex].values[18] = engine.Evaluate("fitFrame[fitFrame$p==1,]$PmaxD").AsVector().First().ToString();

                            mVM.RowViewModels[1 + mIndex].values[19] = string.Join(",", xValues);
                            mVM.RowViewModels[1 + mIndex].values[20] = string.Join(",", yValues);
                        }
                        else
                        {
                            for (int i = 2; i <= 18; i++)
                            {
                                mVM.RowViewModels[1 + mIndex].values[i] = "NA";
                            }

                            mVM.RowViewModels[1 + mIndex].values[19] = string.Join(",", xValues);
                            mVM.RowViewModels[1 + mIndex].values[20] = string.Join(",", yValues);
                            mVM.RowViewModels[1 + mIndex].values[21] = "Model did not converge, was a curve actually present?";
                        }

                    }
                    catch (ParseException pe)
                    {
                        Console.WriteLine(pe.ToString());
                    }

                    mWindow.OutputEvents("Computation #" + ((int)mIndex + (int)1) + " of " + wholeRange.GetLength(1) + " Completed!");

                }

                #endregion

            }
            else if (SelectedMode == "Group")
            {

                #region GroupFitting

                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                yRange = new List<double>();
                xRangeShadow = new List<double>();
                List<double> kRange = new List<double>();
                List<double> pRange = new List<double>();

                try
                {
                    for (int mIndex = 0; mIndex < wholeRange.GetLength(1); mIndex++)
                    {
                        for (int i = 0; i < wholeRange.GetLength(0); i++)
                        {
                            if (double.TryParse(wholeRange[i, mIndex], out holder))
                            {
                                yRange.Add(holder);
                                xRangeShadow.Add(xRange[i]);
                                pRange.Add(1);
                            }
                        }
                    }

                    NumericVector yValues = null;
                    NumericVector xValues = null;

                    for (int i = 0; i < yRange.Count; i++)
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
                            kRange.Add(kRanges[i]);
                        }
                    }

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

                    if (xBehavior == XValueDecisions.DoNothing)
                    {
                        // Do nothing different
                    }
                    else if (xBehavior == XValueDecisions.ChangeHundredth)
                    {
                        for (int i = 0; i < xRangeShadow.Count(); i++)
                        {
                            if (xRangeShadow[i] == 0.0)
                            {
                                xRangeShadow[i] = 0.01;
                            }
                        }
                    }

                    indicesToRemove.Clear();

                    xTemp = new List<double>(xRangeShadow);
                    yTemp = new List<double>(yRange);
                    pTemp = new List<double>(pRange);

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

                        yRange = new List<double>(yTemp);
                        xRangeShadow = new List<double>(xTemp);
                        pRange = new List<double>(pTemp);
                    }

                    NumericVector kValues = engine.CreateNumericVector(kRange.ToArray());
                    engine.SetSymbol("kLoad", kValues);

                    NumericVector participantValues = engine.CreateNumericVector(pRange.ToArray());
                    engine.SetSymbol("pLoad", participantValues);

                    if (modelArraySelection == "Exponential")
                    {

                        yValues = engine.CreateNumericVector(yRange.ToArray());
                        engine.SetSymbol("yLoad", yValues);

                        xValues = engine.CreateNumericVector(xRangeShadow.ToArray());
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

                        xValues = engine.CreateNumericVector(xRangeShadow.ToArray());
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

                    // NA's default to true in R.Net
                    if (engine.Evaluate("fitFrame[fitFrame$p==1,]$q0").AsVector().First().ToString() != "True")
                    {
                        mVM.RowViewModels[1].values[0] = "Aggregated Fitting";

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

                    if (modelArraySelection == "Exponential")
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentialGraphingFunction());
                    }
                    else if (modelArraySelection == "Exponentiated")
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentiatedGraphingFunction());
                    }
                }
                catch (ParseException pe)
                {
                    Console.WriteLine(pe.ToString());
                }

                mWindow.OutputEvents("Group computations Completed!");

                #endregion

            }

            mWindow.OutputEvents("Final Calculations Completed!");
            mWin.Show();
        }
    }
}
