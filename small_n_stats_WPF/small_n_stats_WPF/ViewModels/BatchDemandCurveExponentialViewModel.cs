/* 
    Copyright 2016 Shawn Gilroy

    This file is part of Small N Stats.

    Small N Stats is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    Small N Stats is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Small N Stats.  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>.

*/

using RDotNet;
using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System;
using System.Collections.Generic;
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
        public BatchDemandCurveWindow windowRef { get; set; }

        private bool runLinear = false;
        public bool RunLinear
        {
            get { return runLinear; }
            set
            {
                runLinear = value;
                OnPropertyChanged("RunLinear");
            }
        }

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

        public BatchDemandCurveExponentialViewModel()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            GetXRangeCommand = new RelayCommand(param => GetXRange(), param => true);
            GetYRangeCommand = new RelayCommand(param => GetYRange(), param => true);
            GetKRangeCommand = new RelayCommand(param => GetKRange(), param => true);

            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);
            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            modelArraySelection = "Exponential";
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

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

        private void GetKRange()
        {
            DefaultFieldsToGray();

            if (KRangeValues.Length > 0 && !KRangeValues.ToLower().Contains("spreadsheet"))
            {
                for (int i = lowRowK; i <= highRowK; i++)
                {
                    DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), lowColK);
                    mCell.Background = Brushes.Transparent;
                    mCell = null;
                }
            }

            KBrush = Brushes.Yellow;
            KRangeValues = "Select k values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_K;

        }

        private void DataGrid_PreviewMouseUp_K(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            lowRowK = cells.Min(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));
            highRowK = cells.Max(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));

            lowColK = cells.Min(i => i.Column.DisplayIndex);
            highColK = cells.Max(i => i.Column.DisplayIndex);

            if ((highRowK - lowRowK) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_K;

                lowColK = -1;
                lowRowK = -1;
                highColK = -1;
                highRowK = -1;
                MessageBox.Show("Please select a single horizontal row.  You can have many columns, but just one row of them.");

                return;
            }

            if (mWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (System.Windows.Controls.DataGridCellInfo obj in mWindow.dataGrid.SelectedCells)
                {
                    ((DataGridCell)obj.Column.GetCellContent(obj.Item).Parent).Background = Brushes.LightSalmon;
                }
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_K;

            KBrush = Brushes.LightSalmon;
            KRangeValues = DataGridTools.GetColumnName(lowColK) + lowRowK.ToString() + ":" + DataGridTools.GetColumnName(highColK) + highRowK.ToString();

            KValue = "";
        }

        private void GetXRange()
        {
            DefaultFieldsToGray();

            if (XRangeValues.Length > 0 && !XRangeValues.ToLower().Contains("spreadsheet"))
            {
                for (int i = lowRowX; i <= highRowX; i++)
                {
                    DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), lowColX);
                    mCell.Background = Brushes.Transparent;
                    mCell = null;
                }
            }

            XBrush = Brushes.Yellow;
            XRangeValues = "Select delays on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_X;

        }

        private void DataGrid_PreviewMouseUp_X(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            lowRowX = cells.Min(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));
            highRowX = cells.Max(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));

            lowColX = cells.Min(i => i.Column.DisplayIndex);
            highColX = cells.Max(i => i.Column.DisplayIndex);

            if ((highColX - lowColX) > 0)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_X;

                lowColX = -1;
                lowRowX = -1;
                highColX = -1;
                highRowX = -1;
                MessageBox.Show("Please select a single vertical column.  You can have many rows, but just one column of them.");

                return;
            }

            if (mWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (System.Windows.Controls.DataGridCellInfo obj in mWindow.dataGrid.SelectedCells)
                {
                    ((DataGridCell)obj.Column.GetCellContent(obj.Item).Parent).Background = Brushes.LightBlue;
                }
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_X;

            XBrush = Brushes.LightBlue;
            XRangeValues = DataGridTools.GetColumnName(lowColX) + lowRowX.ToString() + ":" + DataGridTools.GetColumnName(highColX) + highRowX.ToString();
        }

        private void GetYRange()
        {
            DefaultFieldsToGray();

            if (YRangeValues.Length > 0 && !YRangeValues.ToLower().Contains("spreadsheet"))
            {
                for (int i = lowRowY; i <= highRowY; i++)
                {
                    DataGridCell mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), lowColY);
                    mCell.Background = Brushes.Transparent;
                    mCell = null;
                }
            }

            YBrush = Brushes.Yellow;
            YRangeValues = "Select values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Y;

        }

        private void DataGrid_PreviewMouseUp_Y(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();

            lowRowY = cells.Min(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));
            highRowY = cells.Max(i => DataGridTools.GetDataGridRowIndex(mWindow.dataGrid, i));

            lowColY = cells.Min(i => i.Column.DisplayIndex);
            highColY = cells.Max(i => i.Column.DisplayIndex);

            if ((highColY - lowColY) < 2)
            {
                DefaultFieldsToGray();

                mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

                lowColY = -1;
                lowRowY = -1;
                highColY = -1;
                highRowY = -1;
                MessageBox.Show("Please select a single vertical column.  You can have many rows, but just one column of them.");

                return;
            }

            if (mWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (System.Windows.Controls.DataGridCellInfo obj in mWindow.dataGrid.SelectedCells)
                {
                    ((DataGridCell)obj.Column.GetCellContent(obj.Item).Parent).Background = Brushes.LightGreen;
                }
            }

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

            YBrush = Brushes.LightGreen;
            YRangeValues = DataGridTools.GetColumnName(lowColY) + lowRowY.ToString() + ":" + DataGridTools.GetColumnName(highColY) + highRowY.ToString();
        }

        private List<double> GetRangedValues(int startRow, int endRow, int column)
        {
            List<double> mRange = new List<double>();

            DataGridCell mCell;
            double test;

            for (int i = startRow; i <= endRow; i++)
            {
                mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), column);

                if (!Double.TryParse((((TextBlock)mCell.Content)).Text.ToString(), out test))
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

        private List<double> GetRangedValuesHorizontal(int startCol, int endCol, int row)
        {
            List<double> mRange = new List<double>();

            DataGridCell mCell;
            double test;

            for (int i = startCol; i <= endCol; i++)
            {
                mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, row), i);

                if (!Double.TryParse((((TextBlock)mCell.Content)).Text.ToString(), out test))
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

        private List<double>[] GetRanged(int startRowX, int endRowX, int columnX, int startRowY, int endRowY, int columnY)
        {
            List<double>[] array = new List<double>[2];
            array[0] = new List<double>();
            array[1] = new List<double>();

            DataGridCell mCellX,
                         mCellY;

            double testX = -1,
                   testY = -1;

            int i = startRowX,
                j = startRowY;

            for (; i <= endRowX && j <= endRowY;)
            {
                mCellX = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), columnX);
                mCellY = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, j), columnY);

                if (Double.TryParse((((TextBlock)mCellX.Content)).Text.ToString(), out testX) &&
                    Double.TryParse((((TextBlock)mCellY.Content)).Text.ToString(), out testY))
                {
                    array[0].Add(testX);
                    array[1].Add(testY);

                }

                i++;
                j++;
            }

            return array;
        }

        /// <summary>
        /// A method for submitting a string-encoded range and returning the value of the cells selected.
        /// </summary>
        /// <param name="range">
        /// List of double values returned for use as delay or value points in Computation
        /// </param>
        public double[,] ParseBulkRange(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            double[,] mDouble = null;
            DataGridCell mCell;
            double test;

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new double[mCols, mRows];

            try
            {

                for (int i = lowRowValue; i <= highRowValue; i++)
                {

                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), j);

                        if (!Double.TryParse((((TextBlock)mCell.Content)).Text.ToString(), out test))
                        {
                            return null;
                        }
                        else
                        {
                            mDouble[j - lowColValue, i - lowRowValue] = test;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return mDouble;
        }

        /// <summary>
        /// A method for submitting a string-encoded range and returning the value of the cells selected.
        /// </summary>
        /// <param name="range">
        /// List of double values returned for use as delay or value points in Computation
        /// </param>
        public string[,] ParseBulkRangeStrings(int lowRowValue, int highRowValue, int lowColValue, int highColValue)
        {
            string[,] mDouble = null;
            DataGridCell mCell;

            int mRows = (highRowValue - lowRowValue) + 1;
            int mCols = (highColValue - lowColValue) + 1;

            mDouble = new string[mCols, mRows];

            try
            {

                for (int i = lowRowValue; i <= highRowValue; i++)
                {

                    for (int j = lowColValue; j <= highColValue; j++)
                    {
                        mCell = DataGridTools.GetDataGridCell(mWindow.dataGrid, DataGridTools.GetDataGridRow(mWindow.dataGrid, i), j);
                        mDouble[j - lowColValue, i - lowRowValue] = (((TextBlock)mCell.Content)).Text.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

            return mDouble;
        }

        private void CalculateScores()
        {
            if (failed) return;

            bool customK = false;

            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Checking user-supplied ranges and reference points.");
            
            List<double> xRange = new List<double>();
            xRange = GetRangedValues(lowRowX, highRowX, lowColX);

            if (xRange == null)
            {
                mWindow.OutputEvents("Error while validating the Delays.  There cannot be any blank, null or non-numeric fields.");
                MessageBox.Show("Please review the the Delays column.  There cannot be any blank, null or non-numeric fields.");
                return;
            }

            mWindow.OutputEvents("---------------------------------------------------");

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            List<double> yRange = new List<double>();

            string[,] wholeRange = ParseBulkRangeStrings(lowRowY, highRowY, lowColY, highColY);

            if (wholeRange == null)
            {
                mWindow.OutputEvents("There were items that failed validation in the Indifference Point values.  Are any fields blank or not numeric?");
                MessageBox.Show("There were items that failed validation in the Indifference Point values.");
                return;
            }

            List<double> xRangeShadow = new List<double>();
            double holder;

            yRange.Clear();
            xRangeShadow.Clear();

            for (int i = 0; i < wholeRange.GetLength(1); i++)
            {
                if (double.TryParse(wholeRange[0, i], out holder))
                {
                    yRange.Add(holder);
                    xRangeShadow.Add(xRange[i]);
                }
            }
            
            List<double> kRanges = GetRangedValuesHorizontal(lowColK, highColK, lowRowK);

            if (kRanges != null && kRanges.Count() > 2)
            {
                customK = true;

                foreach (double v in kRanges)
                {
                    System.Console.WriteLine("output: " + v.ToString());
                }

            }
            else if ((kRanges.Count() != wholeRange.GetLength(1)))
            {
                mWindow.OutputEvents("Your custom k ranges don't match the # of columns. Column #=" + kRanges.Count() + "; Consumption Dim #=" + wholeRange.GetLength(1));
                MessageBox.Show("Hmm, check your k range.  It doesn't seem paired up");
                return;
            }
            else if (!double.TryParse(KValue, out kValueDouble) && (kValueDouble <= 0.0))
            {
                mWindow.OutputEvents("Error in computing ranges, inputs must be equal in length.");
                mWindow.OutputEvents("Scaling constant (k) was: " + kValueDouble);
                MessageBox.Show("Hmm, check your ranges.  These don't seem paired up");
                return;
            }



            mWindow.OutputEvents("All inputs passed verification.");
            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Beginning Batched Computations...");

            var mWin = new ResultsWindow();
            var mVM = new ResultsViewModel();
            mWin.DataContext = mVM;
            //mWin.Owner = windowRef;
            mWin.Topmost = true;

            for (int i = 0; i < 35; i++)
            {
                mVM.RowViewModels.Add(new RowViewModel());
            }

            for (int mIndex = 0; mIndex < wholeRange.GetLength(0); mIndex++)
            {
                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                yRange.Clear();
                xRangeShadow.Clear();

                for (int i = 0; i < wholeRange.GetLength(1); i++)
                {

                    if (double.TryParse(wholeRange[mIndex, i], out holder))
                    {
                        yRange.Add(holder);
                        xRangeShadow.Add(xRange[i]);
                    }
                }

                try
                {
                    NumericVector yValues = engine.CreateNumericVector(yRange.ToArray());
                    engine.SetSymbol("yLoad", yValues);

                    NumericVector xValues = engine.CreateNumericVector(xRange.ToArray());
                    engine.SetSymbol("xLoad", xValues);

                    List<double> kRange = new List<double>();
                    List<double> pRange = new List<double>();

                    for (int i = 0; i < xRange.Count; i++)
                    {
                        if (customK)
                        {
                            kRange.Add(kRanges[mIndex]);
                        }
                        else
                        {
                            kRange.Add(kValueDouble);
                        }

                        pRange.Add(1);
                    }

                    NumericVector participantValues = engine.CreateNumericVector(pRange.ToArray());
                    engine.SetSymbol("pLoad", participantValues);


                    NumericVector kValues = engine.CreateNumericVector(kRange.ToArray());
                    engine.SetSymbol("kLoad", kValues);

                    if (modelArraySelection == "Exponential")
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunction());
                    }
                    else if (modelArraySelection == "Exponentiated")
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentiatedDemandFunction());
                    }

                    if (mIndex == 0)
                    {
                        mVM.RowViewModels[0].values[0] = "Results of Fitting";
                        mVM.RowViewModels[1].values[0] = "K Value";
                        mVM.RowViewModels[2].values[0] = "Fitted Parameters";
                        mVM.RowViewModels[3].values[0] = "q0";
                        mVM.RowViewModels[4].values[0] = "alpha";

                        mVM.RowViewModels[6].values[0] = "Standard Error";

                        mVM.RowViewModels[7].values[0] = "q0 (se)";
                        mVM.RowViewModels[8].values[0] = "alpha (se)";
                        mVM.RowViewModels[10].values[0] = "95% CI";
                        mVM.RowViewModels[11].values[0] = "Q0";
                        mVM.RowViewModels[12].values[0] = "alpha";

                        mVM.RowViewModels[14].values[0] = "Goodness of Fitness";
                        mVM.RowViewModels[15].values[0] = "R-Squared";
                        mVM.RowViewModels[16].values[0] = "Abs. Sum Squares";
                        mVM.RowViewModels[17].values[0] = "Resid. SD";


                    }

                    mVM.RowViewModels[0].values[1 + mIndex] = "Series #" + (mIndex+1).ToString();
                    mVM.RowViewModels[1].values[1 + mIndex] = kRange.Min().ToString();
                    mVM.RowViewModels[3].values[1 + mIndex] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0").AsVector().First().ToString();
                    mVM.RowViewModels[4].values[1 + mIndex] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alpha").AsVector().First().ToString();

                    mVM.RowViewModels[7].values[1 + mIndex] = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0err").AsVector().First().ToString();
                    mVM.RowViewModels[8].values[1 + mIndex] = engine.Evaluate("fitFrame[fitFrame$p==1,]$alphaerr").AsVector().First().ToString();

                    string qLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0low").AsVector().First().ToString();
                    string qHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$q0high").AsVector().First().ToString();

                    string aLow = engine.Evaluate("fitFrame[fitFrame$p==1,]$alow").AsVector().First().ToString();
                    string aHigh = engine.Evaluate("fitFrame[fitFrame$p==1,]$ahigh").AsVector().First().ToString();

                    mVM.RowViewModels[11].values[1 + mIndex] = qLow + " - " + qHigh;
                    mVM.RowViewModels[12].values[1 + mIndex] = aLow + " - " + aHigh;

                    mVM.RowViewModels[15].values[1 + mIndex] = engine.Evaluate("fitFrame[fitFrame$p==1,]$r2").AsVector().First().ToString();
                    mVM.RowViewModels[16].values[1 + mIndex] = engine.Evaluate("fitFrame[fitFrame$p==1,]$absSS").AsVector().First().ToString();
                    mVM.RowViewModels[17].values[1 + mIndex] = engine.Evaluate("fitFrame[fitFrame$p==1,]$sdResid").AsVector().First().ToString();


                }
                catch (ParseException pe)
                {
                    Console.WriteLine(pe.ToString());
                }

                mWindow.OutputEvents("Computation #" + ((int)mIndex + (int)1) + " of " + wholeRange.GetLength(0) + " Completed!");

            }

            mWindow.OutputEvents("Final Calculations Completed!");
            mWin.Show();

        }
    }
}
