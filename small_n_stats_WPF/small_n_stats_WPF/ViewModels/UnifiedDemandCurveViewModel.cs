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
using small_n_stats_WPF.View;
using small_n_stats_WPF.ViewModel;
using small_n_stats_WPF.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
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

        private string figureDestination = string.Empty;
        private string FigureDestination
        {
            get
            {
                return string.IsNullOrEmpty(figureDestination) ? Conventions.RNull : "'" + figureDestination.Replace("\\", "/") + "'";
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

                if (value)
                {
                    FolderBrowserDialog mDialog = new FolderBrowserDialog();

                    if (mDialog.ShowDialog() == DialogResult.OK)
                    {
                        figureDestination = mDialog.SelectedPath + "\\";
                    }
                    else
                    {
                        figureDestination = string.Empty;
                        outputFigures = false;
                        OnPropertyChanged("OutputFigures");
                    }
                }
                else
                {
                    figureDestination = string.Empty;
                }
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

        #endregion

        #region Commands

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand GetXRangeCommand { get; set; }
        public RelayCommand GetYRangeCommand { get; set; }
        public RelayCommand GetKRangeCommand { get; set; }

        public RelayCommand CalculateScoresCommand { get; set; }
        public RelayCommand AdvancedSettings { get; set; }

        public RelayCommand ConsumptionRangeCommand { get; set; }
        public RelayCommand PricingRangeCommand { get; set; }

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

            CalculateScoresCommand = new RelayCommand(param => CalculateScores(), param => true);
            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            ConsumptionRangeCommand = new RelayCommand(param => UpdateConsumptionRange(), param => true);
            PricingRangeCommand = new RelayCommand(param => UpdatePricingRange(), param => true);

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
        }

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateSelectionMode()
        {
            lowRowX = highRowX = lowColX = highColX = -1;
            lowRowY = highRowY = lowColY = highColY = -1;

            XRangeValues = "";
            YRangeValues = "";

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
                            sNum--;
                            fNum--;

                            XBrush = Brushes.LightBlue;
                            XRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                            lowColX = DataGridTools.GetColumnIndex(firstChars);
                            highColX = DataGridTools.GetColumnIndex(secondChars);

                            lowRowX = fNum;
                            highRowX = sNum;
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Please ensure that only a single row is selected");
                        }
                    }
                    else if (ColumnModeRadio)
                    {
                        if ((DataGridTools.GetColumnIndex(secondChars) - DataGridTools.GetColumnIndex(firstChars)) == 0)
                        {

                            sNum--;
                            fNum--;

                            XBrush = Brushes.LightBlue;
                            XRangeValues = firstChars + firstNums + ":" + secondChars + secondNums;

                            lowColX = DataGridTools.GetColumnIndex(firstChars);
                            highColX = DataGridTools.GetColumnIndex(secondChars);

                            lowRowX = fNum;
                            highRowX = sNum;
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Please ensure that only a single column is selected");
                        }
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Parse error!");
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
                    System.Windows.MessageBox.Show("Parse error!");
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

                System.Windows.MessageBox.Show("Modules for R were not found.  Please connect to the internet and restart the program.");
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
        }

        /// <summary>
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetXRange()
        {
            if (App.IsSearchingForPick)
            {
                return;
            }

            App.IsSearchingForPick = true;

            App.Workbook.PickRange((inst, range) =>
            {
                if (ColumnModeRadio)
                {
                    if (range.Cols < 1)
                    {
                        System.Windows.MessageBox.Show("Please add at least 3 series to the batch");

                        lowColX = -1;
                        lowRowX = -1;
                        highColX = -1;
                        highRowX = -1;

                        App.Workbook.EndPickRange();
                        App.IsSearchingForPick = false;

                        DefaultFieldsToGray();

                        return false;
                    }
                }
                else if (RowModeRadio)
                {
                    if (range.Rows < 1)
                    {
                        System.Windows.MessageBox.Show("Please add at least 3 series to the batch");

                        lowColX = -1;
                        lowRowX = -1;
                        highColX = -1;
                        highRowX = -1;

                        App.Workbook.EndPickRange();
                        App.IsSearchingForPick = false;

                        DefaultFieldsToGray();

                        return false;
                    }
                }

                XBrush = Brushes.LightBlue;
                XRangeValues = DataGridTools.GetColumnName(range.Col) + range.Row.ToString() + ":" + DataGridTools.GetColumnName(range.EndCol) + range.EndRow.ToString();

                lowColX = range.Col;
                lowRowX = range.Row;
                highColX = range.EndCol;
                highRowX = range.EndRow;

                App.IsSearchingForPick = false;

                DefaultFieldsToGray();

                return true;

            }, System.Windows.Input.Cursors.Hand);
            
            DefaultFieldsToGray();

            XBrush = Brushes.Yellow;
            XRangeValues = "Select pricing values on spreadsheet";
        }

        /// <summary>
        /// Successful (or failing) selections result in a range string in respective text fields for later parsing.
        /// </summary>
        private void GetYRange()
        {
            if (App.IsSearchingForPick)
            {
                return;
            }

            App.IsSearchingForPick = true;

            App.Workbook.PickRange((inst, range) =>
            {
                if (ColumnModeRadio)
                {
                    if (range.Cols < 1)
                    {
                        System.Windows.MessageBox.Show("Please add at least 3 series to the batch");

                        lowColY = -1;
                        lowRowY = -1;
                        highColY = -1;
                        highRowY = -1;

                        App.Workbook.EndPickRange();
                        App.IsSearchingForPick = false;

                        DefaultFieldsToGray();

                        return false;
                    }
                }
                else if (RowModeRadio)
                {
                    if (range.Rows < 1)
                    {
                        System.Windows.MessageBox.Show("Please add at least 3 series to the batch");

                        lowColY = -1;
                        lowRowY = -1;
                        highColY = -1;
                        highRowY = -1;

                        App.Workbook.EndPickRange();
                        App.IsSearchingForPick = false;

                        DefaultFieldsToGray();

                        return false;
                    }
                }

                YBrush = Brushes.LightGreen;
                YRangeValues = DataGridTools.GetColumnName(range.Col) + range.Row.ToString() + ":" + DataGridTools.GetColumnName(range.EndCol) + range.EndRow.ToString();

                lowColY = range.Col;
                lowRowY = range.Row;
                highColY = range.EndCol;
                highRowY = range.EndRow;

                App.IsSearchingForPick = false;

                DefaultFieldsToGray();

                return true;

            }, System.Windows.Input.Cursors.Hand);

            DefaultFieldsToGray();

            YBrush = Brushes.Yellow;
            YRangeValues = "Select consumption values on spreadsheet";
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
                    array = DataGridTools.GetRangedValuesHorizontal(lowColX, highColX, lowRowX, lowColY, highColY, lowRowY);

                    xRange = new List<double>(array[0]);
                    yRange = new List<double>(array[1]);

                    dat = CreateDataFrame(engine, null, xRange, yRange);
                    engine.SetSymbol(Conventions.NamedDataFrame, dat);
                }
                else
                {
                    var tmpXRange = DataGridTools.GetRangedValuesVM(lowColX, highColX, lowRowX);
                    string[,] wholeRange = DataGridTools.ParseBulkRangeStringsVM(lowRowY, highRowY, lowColY, highColY);

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
                    array = DataGridTools.GetRangedValuesVertical(lowRowX, highRowX, lowColX, lowRowY, highRowY, lowColY);

                    xRange = new List<double>(array[0]);
                    yRange = new List<double>(array[1]);

                    dat = CreateDataFrame(engine, null, xRange, yRange);
                    engine.SetSymbol(Conventions.NamedDataFrame, dat);
                }
                else
                {
                    var tmpXRange = DataGridTools.GetRangedValuesVerticalVM(lowRowX, highRowX, lowColX);
                    string[,] wholeRange = DataGridTools.ParseBulkRangeStringsVerticalVM(lowRowY, highRowY, lowColY, highColY);

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
                evaluateString = string.Format("{0} <- FitCurves(dat = {1}, equation = {2}, k = {3}, remq0e = {4}, replfree = {5}, rem0 = {6}, nrepl = {7}, replnum = {8}, plotcurves = {9}, vartext = {10}, plotdestination = {11})",
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
                    Conventions.RNull,
                    FigureDestination);
            }
            else
            {
                evaluateString = string.Format("{0} <- FitCurves({1}, {2}, plotcurves = {3}, plotdestination = {4})",
                    Conventions.FittedDataFrame,
                    Conventions.NamedDataFrame,
                    ModelCodes,
                    FigureTag,
                    FigureDestination);
            }

            try
            {
                mWindow.OutputEvents(">>>" + evaluateString);

                engine.Evaluate(evaluateString);

                DataFrame fittedDataFrame = engine.Evaluate(Conventions.FittedDataFrame).AsDataFrame();

                string[] rColNames = fittedDataFrame.ColumnNames;
                string[] rRowNames = fittedDataFrame.RowNames;

                var mResultsWindow = new ResultsGridWindow();
                var mResultsVM = new ViewModelResultsWindow
                {
                    ResultsBook = mResultsWindow.reoGridControl,
                };
                mResultsWindow.DataContext = mResultsVM;
                mResultsWindow.Width = 800;
                mResultsWindow.Height = 600;

                mResultsVM.ResultsBook.CurrentWorksheet.AppendRows(nRows + 10);
                mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(0, 0).Data = "Results of Fitting";

                // Output general metrics

                for (int i=0; i < rColNames.Length; i++)
                {
                    mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(1, i).Data = rColNames[i].Trim();
                }

                for (int i = 0; i < rRowNames.Length; i++)
                {
                    for (int j = 0; j < rColNames.Length; j++)
                    {
                        mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(2 + i, j).Data = fittedDataFrame[i, j].ToString().Trim();
                    }
                }

                // Output stein metrics, skip redundant participant field

                for (int i=0; i < sColNames.Length; i++)
                {
                    mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(1, i + rColNames.Length).Data = sColNames[i].Trim();
                }

                for (int i = 0; i < sRowNames.Length; i++)
                {
                    for (int j = 0; j < sColNames.Length; j++)
                    {
                        mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(2 + i, j + rColNames.Length).Data = steinMetrics[i, j].ToString().Trim();
                    }
                }

                // Output descriptives

                for (int i = 0; i < dColNames.Length; i++)
                {
                    if (i == 0)
                    {
                        mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(2 + sRowNames.Length + 1, 0).Data = "Unit Price Point";
                    }

                    mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(2 + sRowNames.Length + 1, i + 1).Data = dColNames[i].Trim();
                }

                for (int i = 0; i < dRowNames.Length; i++)
                {
                    mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(2 + sRowNames.Length + 2 + i, 0).Data = dRowNames[i].Trim();

                    for (int j = 0; j < dColNames.Length; j++)
                    {
                        mResultsVM.ResultsBook.CurrentWorksheet.CreateAndGetCell(2 + sRowNames.Length + 2 + i, j + 1).Data = descriptiveMetrics[i, j].ToString().Trim();
                    }
                }

                for (int colCount = 0; colCount < rColNames.Length + sColNames.Length; colCount++)
                {
                    mResultsVM.ResultsBook.CurrentWorksheet.AutoFitColumnWidth(colCount, false);
                }

                mResultsWindow.Show();
            }
            catch (Exception e)
            {
                mWindow.OutputEvents(e.ToString());
            }
        }
    }
}
