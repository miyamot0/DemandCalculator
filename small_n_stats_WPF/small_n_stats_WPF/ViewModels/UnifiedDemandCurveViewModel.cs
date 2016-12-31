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
using small_n_stats_WPF.Tags;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace small_n_stats_WPF.ViewModels
{
    class UnifiedDemandCurveViewModel : BaseViewModel
    {
        public MainWindow mWindow { get; set; }
        public DemandCurveUnifiedWindow windowRef { get; set; }

        #region DataModes

        private bool rowModeRadio = false;
        public bool RowModeRadio
        {
            get { return rowModeRadio; }
            set
            {
                rowModeRadio = value;
                OnPropertyChanged("RowModeRadio");
                UpdateSelectionMode();
            }
        }

        private bool columnModeRadio = false;
        public bool ColumnModeRadio
        {
            get { return columnModeRadio; }
            set
            {
                columnModeRadio = value;
                OnPropertyChanged("ColumnModeRadio");
                UpdateSelectionMode();
            }
        }

        #endregion

        #region ModelModes

        private string ModelCodes
        {
            get
            {
                if (HurshModel)
                {
                    return Conventions.ExponentialModel;
                }
                else if (KoffarnusModel)
                {
                    return Conventions.ExponentiatedModel;
                }
                else
                {
                    return Conventions.LinearModel;
                }
            }
        }

        private bool hurshModel = false;
        public bool HurshModel
        {
            get { return hurshModel; }
            set
            {
                hurshModel = value;
                OnPropertyChanged("HurshModel");
                UpdateSelectors();
            }
        }

        private bool koffarnusModel = false;
        public bool KoffarnusModel
        {
            get { return koffarnusModel; }
            set
            {
                koffarnusModel = value;
                OnPropertyChanged("KoffarnusModel");
                UpdateSelectors();
            }
        }

        private bool linearModel = false;
        public bool LinearModel
        {
            get { return linearModel; }
            set
            {
                linearModel = value;
                OnPropertyChanged("LinearModel");
                UpdateSelectors();
            }
        }

        #endregion

        #region Q0e Settings

        private string RemQ0ETag
        {
            get
            {
                if (DropXValues)
                {
                    return Conventions.RFalse;
                }
                else
                {
                    return Conventions.RTrue;
                }
            }
        }

        private bool dropXValues = false;
        public bool DropXValues
        {
            get { return dropXValues; }
            set
            {
                dropXValues = value;
                OnPropertyChanged("DropXValues");
            }
        }

        private bool keepXValues = false;
        public bool KeepXValues
        {
            get { return keepXValues; }
            set
            {
                keepXValues = value;
                OnPropertyChanged("KeepXValues");
            }
        }

        #endregion

        #region Q0e Replacement Settings

        private string ReplFreeTag
        {
            get
            {
                if (ModifyXValues)
                {
                    return modifyQ0Value;
                }
                else
                {
                    return Conventions.RNull;
                }
            }
        }

        private bool modifyXValues = false;
        public bool ModifyXValues
        {
            get { return modifyXValues; }
            set
            {
                modifyXValues = value;
                OnPropertyChanged("ModifyXValues");
            }
        }

        private string modifyQ0Value = "";
        public string ModifyQ0Value
        {
            get { return modifyQ0Value; }
            set
            {
                modifyQ0Value = value;
                OnPropertyChanged("ModifyQ0Value");
            }
        }

        #endregion

        #region Y Settings

        private string RemoveZeroTag
        {
            get
            {
                if (DropYValues)
                {
                    return Conventions.RTrue;
                }
                else
                {
                    return Conventions.RFalse;
                }
            }
        }

        private bool dropYValues = false;
        public bool DropYValues
        {
            get { return dropYValues; }
            set
            {
                dropYValues = value;
                OnPropertyChanged("DropYValues");
                UpdateYSelectors();
            }
        }

        private bool keepYValues = false;
        public bool KeepYValues
        {
            get { return keepYValues; }
            set
            {
                keepYValues = value;
                OnPropertyChanged("KeepYValues");
                UpdateYSelectors();
            }
        }

        #endregion

        #region Y Replacement Settings

        private string ReplaceNumberTag
        {
            get
            {
                if (ModYValues)
                {
                    if (ModYValueTenth)
                    {
                        return "0.1";
                    }
                    else if (ModYValuesHundredth)
                    {
                        return "0.01";
                    }
                    else if (ModYValuesCustom)
                    {
                        return YModValues;
                    }
                    else
                    {
                        return "0.1";
                    }
                }
                else
                {
                    return Conventions.RNull;
                }
            }
        }

        private bool modYValues = false;
        public bool ModYValues
        {
            get { return modYValues; }
            set
            {
                modYValues = value;
                OnPropertyChanged("ModYValues");
                UpdateYSelectors();
            }
        }

        private bool modYValueTenth = false;
        public bool ModYValueTenth
        {
            get { return modYValueTenth; }
            set
            {
                modYValueTenth = value;
                OnPropertyChanged("ModYValueTenth");
            }
        }

        private bool modYValuesHundredth = false;
        public bool ModYValuesHundredth
        {
            get { return modYValuesHundredth; }
            set
            {
                modYValuesHundredth = value;
                OnPropertyChanged("ModYValuesHundredth");
            }
        }

        private bool modYValuesCustom = false;
        public bool ModYValuesCustom
        {
            get { return modYValuesCustom; }
            set
            {
                modYValuesCustom = value;
                OnPropertyChanged("ModYValuesCustom");
            }
        }

        private string yModValues = "";
        public string YModValues
        {
            get { return yModValues; }
            set
            {
                yModValues = value;
                OnPropertyChanged("YModValues");
            }
        }

        #endregion

        #region Figure Outputs

        private string FigureTag
        {
            get
            {
                return (OutputFigures) ? Conventions.RTrue : Conventions.RFalse;
            }
        }

        private bool outputFigures = false;
        public bool OutputFigures
        {
            get { return outputFigures; }
            set
            {
                outputFigures = value;
                OnPropertyChanged("OutputFigures");
            }
        }

        #endregion

        #region K Settings

        private string KCodes
        {
            get
            {
                if (IndividualizedKValue)
                {
                    return Conventions.IndividualK;
                }
                else if (FitKValue)
                {
                    return Conventions.FitK;
                }
                else if (SharedKValue)
                {
                    return Conventions.ShareK;
                }
                else
                {
                    return CustomKValueField;
                }
            }
        }

        private bool individualizedKValue = false;
        public bool IndividualizedKValue
        {
            get { return individualizedKValue; }
            set
            {
                individualizedKValue = value;
                OnPropertyChanged("IndividualizedKValue");
            }
        }

        private bool fitKValue = false;
        public bool FitKValue
        {
            get { return fitKValue; }
            set
            {
                fitKValue = value;
                OnPropertyChanged("FitKValue");
            }
        }

        private bool sharedKValue = false;
        public bool SharedKValue
        {
            get { return sharedKValue; }
            set
            {
                sharedKValue = value;
                OnPropertyChanged("SharedKValue");
            }
        }

        private bool customKValue = false;
        public bool CustomKValue
        {
            get { return customKValue; }
            set
            {
                customKValue = value;
                OnPropertyChanged("CustomKValue");
            }
        }

        private string customKValueField = "";
        public string CustomKValueField
        {
            get { return customKValueField; }
            set
            {
                customKValueField = value;
                OnPropertyChanged("CustomKValueField");
            }
        }

        #endregion

        #region UIBindings   
  
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

        private string gRangeValues = "";
        public string GRangeValues
        {
            get { return gRangeValues; }
            set
            {
                gRangeValues = value;
                OnPropertyChanged("GRangeValues");
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

        private Brush gBrush = Brushes.White;
        public Brush GBrush
        {
            get { return gBrush; }
            set
            {
                gBrush = value;
                OnPropertyChanged("GBrush");
            }
        }

        #endregion

        #region Math

        REngine engine;

        bool failed;
        bool loaded = false;

        int lowRowX = -1,
            highRowX = -1,
            lowColX = -1,
            highColX = -1;

        int lowRowY = -1,
            highRowY = -1,
            lowColY = -1,
            highColY = -1;

        int lowRowG = -1,
            highRowG = -1,
            lowColG = -1,
            highColG = -1;

        #endregion

        #region Commands

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand GetXRangeCommand { get; set; }
        public RelayCommand GetYRangeCommand { get; set; }
        public RelayCommand GetGRangeCommand { get; set; }
        public RelayCommand GetKRangeCommand { get; set; }

        public RelayCommand CalculateScoresCommand { get; set; }
        public RelayCommand AdvancedSettings { get; set; }

        public RelayCommand ConsumptionRangeCommand { get; set; }
        public RelayCommand PricingRangeCommand { get; set; }
        public RelayCommand GroupingRangeCommand { get; set; }

        #endregion

        private List<string> filesList = new List<string>();

        /// <summary>
        /// Public constructor
        /// </summary>
        public UnifiedDemandCurveViewModel()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            GetXRangeCommand = new RelayCommand(param => GetXRange(), param => true);
            GetYRangeCommand = new RelayCommand(param => GetYRange(), param => true);
            GetGRangeCommand = new RelayCommand(param => GetGRange(), param => true);

            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);
            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            ConsumptionRangeCommand = new RelayCommand(param => UpdateConsumptionRange(), param => true);
            PricingRangeCommand = new RelayCommand(param => UpdatePricingRange(), param => true);
            GroupingRangeCommand = new RelayCommand(param => UpdateGroupingRange(), param => true);

            HurshModel = true;
            RowModeRadio = true;
        }

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateButtons()
        {
            lowRowY = highRowY = lowColY = highColY = -1;
            YBrush = Brushes.LightGray;
            YRangeValues = "";

            lowRowG = highRowG = lowColG = highColG = -1;
            YBrush = Brushes.LightGray;
            GRangeValues = "";

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;
        }

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateSelectionMode()
        {
            lowRowX = highRowX = lowColX = highColX = -1;
            lowRowY = highRowY = lowColY = highColY = -1;
            lowRowG = highRowG = lowColG = highColG = -1;

            XRangeValues = "";
            YRangeValues = "";
            GRangeValues = "";

            DefaultFieldsToGray();
        }

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateSelectors()
        {
            if (HurshModel || LinearModel)
            {
                DropXValues = true;
                DropYValues = true;
            }
            else if (KoffarnusModel)
            {
                KeepXValues = true;
                KeepYValues = true;
            }
        }

        /// <summary>
        /// Query user for a range
        /// </summary>
        private void UpdatePricingRange()
        {
            var mWin = new RangePrompt();
            mWin.Topmost = true;
            mWin.Owner = windowRef;
            mWin.ResponseText = XRangeValues;
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
                    if (RowModeRadio)
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
                    else if (ColumnModeRadio)
                    {
                        if ((DataGridTools.GetColumnIndex(secondChars) - DataGridTools.GetColumnIndex(firstChars)) == 0)
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
                            MessageBox.Show("Please ensure that only a single column is selected");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Parse error!");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateGroupingRange()
        {
            var mWin = new RangePrompt();
            mWin.Topmost = true;
            mWin.Owner = windowRef;
            mWin.ResponseText = GRangeValues;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (mWin.ShowDialog() == true)
            {
                if (mWin.ResponseText.Trim().Length == 0)
                {
                    GBrush = Brushes.LightGray;
                    GRangeValues = "";
                    lowColG = highColG = lowRowG = highRowG = -1;
                }

                string[] addresses = mWin.ResponseText.Split(':');

                if (addresses.Length != 2) return;

                var firstChars = new String(addresses[0].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var firstNums = new String(addresses[0].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                var secondChars = new String(addresses[1].ToCharArray().Where(c => !Char.IsDigit(c)).ToArray());
                var secondNums = new String(addresses[1].ToCharArray().Where(c => Char.IsDigit(c)).ToArray());

                int fNum, sNum;

                if (int.TryParse(firstNums, out fNum) && int.TryParse(secondNums, out sNum) && firstChars.Length > 0 && secondChars.Length > 0)
                {
                    if (RowModeRadio)
                    {
                        if ((DataGridTools.GetColumnIndex(firstChars) - DataGridTools.GetColumnIndex(secondChars)) == 0)
                        {
                            GBrush = Brushes.LightSalmon;
                            GRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                            lowColG = DataGridTools.GetColumnIndex(firstChars);
                            highColG = DataGridTools.GetColumnIndex(secondChars);

                            lowRowG = fNum;
                            highRowG = sNum;
                        }
                        else
                        {
                            MessageBox.Show("Please ensure that only a single column is selected");
                        }
                    }
                    else if (ColumnModeRadio)
                    {
                        if ((sNum - fNum) == 0)
                        {
                            GBrush = Brushes.LightSalmon;
                            GRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                            lowColG = DataGridTools.GetColumnIndex(firstChars);
                            highColG = DataGridTools.GetColumnIndex(secondChars);

                            lowRowG = fNum;
                            highRowG = sNum;
                        }
                        else
                        {
                            MessageBox.Show("Please ensure that only a single row is selected");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Parse error!");
                }
            }
        }

        /// <summary>
        /// Update interface
        /// </summary>
        private void UpdateYSelectors()
        {
            if (ModYValues)
            {
                windowRef.yModTenth.IsEnabled = true;
                windowRef.yModHundredth.IsEnabled = true;
                windowRef.yModCustom.IsEnabled = true;
                windowRef.YsettingsCustom.IsEnabled = true;
            }
            else
            {
                if (loaded)
                {
                    windowRef.yModTenth.IsChecked = false;
                    windowRef.yModHundredth.IsChecked = false;
                    windowRef.yModCustom.IsChecked = false;
                    YModValues = "";

                    windowRef.yModTenth.IsEnabled = false;
                    windowRef.yModHundredth.IsEnabled = false;
                    windowRef.yModCustom.IsEnabled = false;
                    windowRef.YsettingsCustom.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Update interface
        /// </summary>
        private void UpdateKSelectors()
        {
            if (loaded)
            {
                if (CustomKValue)
                {
                    windowRef.kSettingsCustom.IsEnabled = true;
                }
                else
                {
                    windowRef.kSettingsCustom.IsEnabled = true;
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
            mWin.ResponseText = YRangeValues;
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
                    YBrush = Brushes.LightGreen;
                    YRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                    lowColY = DataGridTools.GetColumnIndex(firstChars);
                    highColY = DataGridTools.GetColumnIndex(secondChars);

                    lowRowY = fNum;
                    highRowY = sNum;
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

            if (filesList.Count > 0)
            {
                foreach (string str in filesList)
                {
                    if (File.Exists(str))
                    {
                        File.Delete(str);
                    }
                }
            }
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

                HurshModel = true;
                loaded = true;

                UpdateYSelectors();
                UpdateKSelectors();

                IndividualizedKValue = true;
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
                AdvancedMenu = !AdvancedMenu;
                if (HurshModel)
                {
                    windowRef.hurshRadioButton.IsChecked = true;
                }
                else
                {
                    windowRef.koffRadioButton.IsChecked = true;
                }
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

            if (GRangeValues.Length < 1 || GRangeValues.ToLower().Contains("spreadsheet"))
            {
                GBrush = Brushes.LightGray;
                GRangeValues = string.Empty;
            }
        }

        /// <summary>
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetXRange()
        {
            mWindow.dataGrid.CommitEdit();

            DefaultFieldsToGray();

            XBrush = Brushes.Yellow;
            XRangeValues = "Select pricing values on spreadsheet";

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

            if (RowModeRadio)
            {
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
            }
            else if (ColumnModeRadio)
            {
                if ((highColX - lowColX) > 0)
                {
                    DefaultFieldsToGray();

                    mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_X;

                    lowColX = -1;
                    lowRowX = -1;
                    highColX = -1;
                    highRowX = -1;
                    MessageBox.Show("Please select a single vertical column.  You can have many rows, but just one column of pricing values.");

                    return;
                }
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
            mWindow.dataGrid.CommitEdit();

            DefaultFieldsToGray();

            YBrush = Brushes.Yellow;
            YRangeValues = "Select consumption values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_Y;
        }

        /// <summary>
        /// Delegate after highlighting takes place on datagrid (call back specific to delays).
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

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

            YBrush = Brushes.LightGreen;
            YRangeValues = DataGridTools.GetColumnName(lowColY) + lowRowY.ToString() + ":" + DataGridTools.GetColumnName(highColY) + highRowY.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GetGRange()
        {
            mWindow.dataGrid.CommitEdit();

            DefaultFieldsToGray();

            GBrush = Brushes.Yellow;
            GRangeValues = "Select consumption values on spreadsheet";

            mWindow.dataGrid.PreviewMouseUp += DataGrid_PreviewMouseUp_G;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_PreviewMouseUp_G(object sender, MouseButtonEventArgs e)
        {
            List<DataGridCellInfo> cells = mWindow.dataGrid.SelectedCells.ToList();
            var itemSource = mWindow.dataGrid.ItemsSource as ObservableCollection<RowViewModel>;

            if (cells.Count < 1 || itemSource.Count < 1) return;

            lowRowG = cells.Min(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowG = cells.Max(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));

            lowColG = cells.Min(i => i.Column.DisplayIndex);
            highColG = cells.Max(i => i.Column.DisplayIndex);

            if (RowModeRadio)
            {
                // if selecting rows, only ONE column

                if ((highColG - lowColG) > 0 || (highRowG - lowRowG) < 2)
                {
                    DefaultFieldsToGray();

                    mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_G;

                    lowColG = -1;
                    lowRowG = -1;
                    highColG = -1;
                    highRowG = -1;
                    MessageBox.Show("Please select a matrix of consumption values, with more than one row of values with at least three individual points of data (i.e., 3x3).");

                    return;
                }
            }
            else if (ColumnModeRadio)
            {
                // if selecting columns, only ONE row

                if ((highRowG - lowRowG) > 0 || (highColG - lowColG) < 2)
                {
                    DefaultFieldsToGray();

                    mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_G;

                    lowColG = -1;
                    lowRowG = -1;
                    highColG = -1;
                    highRowG = -1;
                    MessageBox.Show("Please select a matrix of consumption values, with more than one column of values with at least three individual points of data (i.e., 3x3).");

                    return;
                }
            }
 
            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_G;

            GBrush = Brushes.LightSalmon;
            GRangeValues = DataGridTools.GetColumnName(lowColG) + lowRowG.ToString() + ":" + DataGridTools.GetColumnName(highColG) + highRowG.ToString();
        }

        /// <summary>
        /// EXPERIMENTAL, data-frame based input for direct import to beezdemand
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="ids"></param>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <returns></returns>
        private static DataFrame CreateDataFrame(REngine engine, List<double> ids, List<double> xs, List<double> ys)
        {
            IEnumerable[] columns = new IEnumerable[3];

            if (ids == null)
            {
                ids = new List<double>();
                foreach (double value in xs)
                {
                    ids.Add(1);
                }
            }

            columns[0] = ids;
            columns[1] = xs;
            columns[2] = ys;
            string[] columnNames = new string[] { "id", "x", "y" };

            return engine.CreateDataFrame(columns, columnNames: columnNames);
        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateScores()
        {
            mWindow.dataGrid.CommitEdit();

            if (failed) return;

            if (lowColX < 0 || lowColY < 0) return;

            int nRows = -1;

            mWindow.OutputEvents(" ");
            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Checking user-supplied ranges and reference points....");

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            #region Parse datagrid

            List<double>[] array = null;

            List<double> xRange = null, 
                         yRange = null,
                         gRange = null;

            DataFrame dat;

            if (RowModeRadio)
            {
                nRows = highRowY - lowRowY;

                if (nRows == 0)
                {
                    array = DataGridTools.GetRangedValuesHorizontal(lowColX, highColX, lowRowX, lowColY, highColY, lowRowY, mWindow.dataGrid.ItemsSource);

                    xRange = new List<double>(array[0]);
                    yRange = new List<double>(array[1]);

                    dat = CreateDataFrame(engine, null, xRange, yRange);
                    engine.SetSymbol(Conventions.NamedDataFrame, dat);
                }
                else
                {
                    var tmpXRange = DataGridTools.GetRangedValuesVM(lowColX, highColX, lowRowX, mWindow.dataGrid.ItemsSource);
                    string[,] wholeRange = DataGridTools.ParseBulkRangeStringsVM(lowRowY, highRowY, lowColY, highColY, mWindow.dataGrid.ItemsSource);

                    xRange = new List<double>();
                    yRange = new List<double>();
                    gRange = new List<double>();

                    for (int i = 0; i < wholeRange.GetLength(1); i++)
                    {
                        for (int j = 0; j < wholeRange.GetLength(0); j++)
                        {
                            double yVal = double.NaN;
                            double.TryParse(wholeRange[j,i], out yVal);
                            yRange.Add(yVal);

                            xRange.Add(tmpXRange[j]);

                            gRange.Add(i + 1);
                        }
                    }

                    dat = CreateDataFrame(engine, gRange, xRange, yRange);
                    engine.SetSymbol(Conventions.NamedDataFrame, dat);
                }
            }
            else if (ColumnModeRadio)
            {
                nRows = highColY - lowColY;

                if (nRows == 0)
                {
                    array = DataGridTools.GetRangedValuesVertical(lowRowX, highRowX, lowColX, lowRowY, highRowY, lowColY, mWindow.dataGrid.ItemsSource);

                    xRange = new List<double>(array[0]);
                    yRange = new List<double>(array[1]);

                    dat = CreateDataFrame(engine, null, xRange, yRange);
                    engine.SetSymbol(Conventions.NamedDataFrame, dat);
                }
                else
                {
                    var tmpXRange = DataGridTools.GetRangedValuesVerticalVM(lowRowX, highRowX, lowColX, mWindow.dataGrid.ItemsSource);
                    string[,] wholeRange = DataGridTools.ParseBulkRangeStringsVerticalVM(lowRowY, highRowY, lowColY, highColY, mWindow.dataGrid.ItemsSource);

                    Console.WriteLine("whole range 0: " + wholeRange.GetLength(0) + " 1: " + wholeRange.GetLength(1));

                    xRange = new List<double>();
                    yRange = new List<double>();
                    gRange = new List<double>();

                    for (int i = 0; i < wholeRange.GetLength(1); i++)
                    {
                        for (int j = 0; j < wholeRange.GetLength(0); j++)
                        {
                            double yVal = double.NaN;
                            double.TryParse(wholeRange[j, i], out yVal);
                            yRange.Add(yVal);

                            xRange.Add(tmpXRange[j]);

                            gRange.Add(i + 1);
                        }
                    }

                    dat = CreateDataFrame(engine, gRange, xRange, yRange);
                    engine.SetSymbol(Conventions.NamedDataFrame, dat);
                }
            }

            #endregion

            mWindow.OutputEvents("Data passed null and type checks...");
            mWindow.OutputEvents("Determining a fitting heuristic...");

            engine.Evaluate("require(beezdemand)");

            #region Output debug

            if (true)
            {
                // Debug
                engine.Evaluate(string.Format("print({0})", Conventions.NamedDataFrame));
            }

            #endregion

            // Build descriptive output
            #region Descriptives Assessment

            // Invoke methods for descriptives
            engine.Evaluate(string.Format("{0} <- NULL", 
                Conventions.DescriptiveDataFrame));
            engine.Evaluate(string.Format("{0} <- GetDescriptives({1})", 
                Conventions.DescriptiveDataFrame, 
                Conventions.NamedDataFrame));

            DataFrame descriptiveMetrics = engine.Evaluate(Conventions.DescriptiveDataFrame).AsDataFrame();

            string[] dColNames = descriptiveMetrics.ColumnNames;
            string[] dRowNames = descriptiveMetrics.RowNames;

            StringBuilder descriptivesSb = new StringBuilder();

            descriptivesSb.Append("Unit Price".PadRight(14, ' '));

            foreach (string colName in dColNames)
            {
                descriptivesSb.Append(colName.Trim().PadRight(14, ' '));
            }

            for (int i=0; i < dRowNames.Length; i++)
            {
                descriptivesSb.Append('\n' + dRowNames[i].Trim().PadRight(14, ' '));

                for (int j=0; j < dColNames.Length; j++)
                {
                    descriptivesSb.Append(descriptiveMetrics[i, j].ToString().Trim().PadRight(14, ' '));
                }
            }

            // Add spacer
            descriptivesSb.Append("\n");

            #endregion

            // Build stein frame output
            #region Stein Metric

            StringBuilder steinSb = new StringBuilder();

            // TODO stein settings

            // Invoke methods for stein
            engine.Evaluate(string.Format("{0} <- NULL",
                Conventions.SteinDataFrame));
            engine.Evaluate(string.Format("{0} <- CheckUnsystematic({1}, deltaq = 0.025, bounce = 0.1, reversals = 0, ncons0 = 2)",
                Conventions.SteinDataFrame,
                Conventions.NamedDataFrame));

            DataFrame steinMetrics = engine.Evaluate(Conventions.SteinDataFrame).AsDataFrame();

            string[] sColNames = steinMetrics.ColumnNames;
            string[] sRowNames = steinMetrics.RowNames;

            steinSb.Append("\n" + "".PadRight(15, ' '));

            foreach (string colName in sColNames)
            {
                steinSb.Append(colName.Trim().PadRight(15, ' '));
            }

            for (int i = 0; i < sRowNames.Length; i++)
            {
                steinSb.Append('\n' + sRowNames[i].Trim().PadRight(15, ' '));

                for (int j = 0; j < sColNames.Length; j++)
                {
                    steinSb.Append(steinMetrics[i, j].ToString().Trim().PadRight(15, ' '));
                }
            }

            var steinWindow = new CheckWindow(new string[] { "I'd like to proceed", "I'd like to review my data" }, "I'd like to proceed");
            System.Windows.Documents.Paragraph para = new System.Windows.Documents.Paragraph();
            para.Inlines.Add(steinSb.ToString());
            steinWindow.outputWindow.Document.Blocks.Add(para);
            steinWindow.outputWindow.ScrollToEnd();

            steinWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            steinWindow.Title = "Results of Stein Test";
            steinWindow.Owner = windowRef;
            steinWindow.Width = 650;
            steinWindow.Height = 400;

            steinWindow.ShowDialog();

            #endregion

            if (steinWindow.MessageOptions.SelectedValue.ToString().Contains("review") || !steinWindow.GotClicked)
            {
                return;
            }

            string evaluateString = null;

            if (AdvancedMenu)
            {
                //FitCurves <- function(dat, equation, k, remq0e = FALSE, replfree = NULL, rem0 = FALSE, nrepl = NULL, replnum = NULL, plotcurves = FALSE, vartext = NULL)
                evaluateString = string.Format("{0} <- FitCurves(dat = {1}, equation = {2}, k = {3}, remq0e = {4}, replfree = {5}, rem0 = {6}, nrepl = {7}, replnum = {8}, plotcurves = {9}, vartext = {10})",
                    Conventions.FittedDataFrame,
                    Conventions.NamedDataFrame,
                    ModelCodes,
                    KCodes,
                    RemQ0ETag,
                    ReplFreeTag,
                    RemoveZeroTag,
                    Conventions.RNull,
                    ReplaceNumberTag,
                    FigureTag,
                    Conventions.RNull);
            }
            else
            {
                evaluateString = string.Format("{0} <- FitCurves({1}, {2})",
                    Conventions.FittedDataFrame,
                    Conventions.NamedDataFrame,
                    ModelCodes);
            }

            try
            {
                engine.Evaluate(evaluateString);

                if (true)
                {
                    engine.Evaluate(string.Format("print({0})", Conventions.FittedDataFrame));
                }

                DataFrame fittedDataFrame = engine.Evaluate(Conventions.FittedDataFrame).AsDataFrame();

                string[] rColNames = fittedDataFrame.ColumnNames;
                string[] rRowNames = fittedDataFrame.RowNames;

                var mResultsWindow = new ResultsWindow();
                var mResultsVM = new ResultsViewModel();
                mResultsWindow.DataContext = mResultsVM;

                for (int i = 0; i < nRows + 10; i++)
                {
                    mResultsVM.RowViewModels.Add(new RowViewModel());
                }

                mResultsVM.RowViewModels[0].values[0] = "Results of Fitting";

                for (int i=0; i < rColNames.Length; i++)
                {
                    mResultsVM.RowViewModels[1].values[i] = rColNames[i].Trim();
                }

                for (int i = 0; i < rRowNames.Length; i++)
                {
                    for (int j = 0; j < rColNames.Length; j++)
                    {
                        mResultsVM.RowViewModels[2 + i].values[j] = fittedDataFrame[i, j].ToString().Trim();
                    }
                }

                mResultsWindow.Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            mWindow.dataGrid.IsReadOnly = false;
        }
    }
}
