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
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Models;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        #region UIBindings

        #region CalculationModes

        private bool singleModeRadio = false;
        public bool SingleModeRadio
        {
            get { return singleModeRadio; }
            set
            {
                singleModeRadio = value;
                OnPropertyChanged("SingleModeRadio");
                UpdateButtons();
            }
        }

        private bool batchModeRadio = false;
        public bool BatchModeRadio
        {
            get { return batchModeRadio; }
            set
            {
                batchModeRadio = value;
                OnPropertyChanged("BatchModeRadio");
                UpdateButtons();
            }
        }

        #endregion

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

        #region AdvancedBindings

        private bool fitQ0 = false;
        public bool FitQ0
        {
            get { return fitQ0; }
            set
            {
                fitQ0 = value;
                OnPropertyChanged("FitQ0");
                UpdateQ0Selectors();
            }
        }

        private bool fixQ0 = false;
        public bool FixQ0
        {
            get { return fixQ0; }
            set
            {
                fixQ0 = value;
                OnPropertyChanged("FixQ0");
                UpdateQ0Selectors();
            }
        }

        private string fixedQ0Value = "";
        public string FixedQ0Value
        {
            get { return fixedQ0Value; }
            set
            {
                fixedQ0Value = value;
                OnPropertyChanged("FixedQ0Value");
                UpdateQ0Selectors();
            }
        }

        private bool boundQ0 = false;
        public bool BoundQ0
        {
            get { return boundQ0; }
            set
            {
                boundQ0 = value;
                OnPropertyChanged("BoundQ0");
            }
        }

        private string boundQ0ValueLow = "";
        public string BoundQ0ValueLow
        {
            get { return boundQ0ValueLow; }
            set
            {
                boundQ0ValueLow = value;
                OnPropertyChanged("BoundQ0ValueLow");
            }
        }

        private string boundQ0ValueHigh = "";
        public string BoundQ0ValueHigh
        {
            get { return boundQ0ValueHigh; }
            set
            {
                boundQ0ValueHigh = value;
                OnPropertyChanged("BoundQ0ValueHigh");
            }
        }

        private bool fitK = false;
        public bool FitK
        {
            get { return fitK; }
            set
            {
                fitK = value;
                OnPropertyChanged("FitK");
                UpdateKBoundingSelectors();
            }
        }

        private bool boundK = false;
        public bool BoundK
        {
            get { return boundK; }
            set
            {
                boundK = value;
                OnPropertyChanged("BoundK");
                UpdateKBoundingSelectors();
            }
        }

        private string boundKValueLow = "";
        public string BoundKValueLow
        {
            get { return boundKValueLow; }
            set
            {
                boundKValueLow = value;
                OnPropertyChanged("BoundKValueLow");
            }
        }

        private string boundKValueHigh = "";
        public string BoundKValueHigh
        {
            get { return boundKValueHigh; }
            set
            {
                boundKValueHigh = value;
                OnPropertyChanged("BoundKValueHigh");
            }
        }

        #endregion

        #region ModelModes

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

        #endregion

        #region X_Decisions

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

        #region Y_Decisions

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

        #endregion

        #region K_Decisions

        private bool groupEmpirical = false;
        public bool GroupEmpirical
        {
            get { return groupEmpirical; }
            set
            {
                groupEmpirical = value;
                OnPropertyChanged("GroupEmpirical");
                UpdateKSelectors();
            }
        }

        private bool groupEmpiricalAverage = false;
        public bool GroupEmpiricalAverage
        {
            get { return groupEmpiricalAverage; }
            set
            {
                groupEmpiricalAverage = value;
                OnPropertyChanged("GroupEmpiricalAverage");
                UpdateKSelectors();
            }
        }

        private bool groupFitted = false;
        public bool GroupFitted
        {
            get { return groupFitted; }
            set
            {
                groupFitted = value;
                OnPropertyChanged("GroupFitted");
                UpdateKSelectors();
            }
        }

        private bool indivEmpirical = false;
        public bool IndivEmpirical
        {
            get { return indivEmpirical; }
            set
            {
                indivEmpirical = value;
                OnPropertyChanged("IndivEmpirical");
                UpdateKSelectors();
            }
        }

        private bool indivFitted = false;
        public bool IndivFitted
        {
            get { return indivFitted; }
            set
            {
                indivFitted = value;
                OnPropertyChanged("IndivFitted");
                UpdateKSelectors();
            }
        }

        private bool customK = false;
        public bool CustomK
        {
            get { return customK; }
            set
            {
                customK = value;
                OnPropertyChanged("CustomK");
                UpdateKSelectors();
            }
        }

        #endregion

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

        private bool possibleFigures = true;
        public bool PossibleFigures
        {
            get { return possibleFigures; }
            set
            {
                possibleFigures = value;
                OnPropertyChanged("PossibleFigures");
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
        private double kValueAverage = 0;
        private double yModValue = 0;

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

        int lowRowK = -1,
            highRowK = -1,
            lowColK = -1,
            highColK = -1;

        int lowRowG = -1,
            highRowG = -1,
            lowColG = -1,
            highColG = -1;

        string path1 = null, path2 = null;
        double heldMaxY = double.NaN;
        double heldK = double.NaN;

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
        public RelayCommand ConstantRangeCommand { get; set; }
        public RelayCommand ResetConstantRangeCommand { get; set; }

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
            GetKRangeCommand = new RelayCommand(param => GetKRange(), param => true);

            CalculateScoresCommand = new RelayCommand(param => PreScoring(), param => true);
            AdvancedSettings = new RelayCommand(param => UpdateSettings(), param => true);

            ConsumptionRangeCommand = new RelayCommand(param => UpdateConsumptionRange(), param => true);
            PricingRangeCommand = new RelayCommand(param => UpdatePricingRange(), param => true);
            GroupingRangeCommand = new RelayCommand(param => UpdateGroupingRange(), param => true);
            ConstantRangeCommand = new RelayCommand(param => UpdateKRange(), param => true);
            ResetConstantRangeCommand = new RelayCommand(param => ResetKRange(), param => true);

            HurshModel = true;
            RowModeRadio = true;
        }

        /// <summary>
        /// Calls to update after mode change
        /// </summary>
        private void UpdateButtons()
        {
            OutputFigures = false;
            lowRowY = highRowY = lowColY = highColY = -1;
            YBrush = Brushes.LightGray;
            YRangeValues = "";

            lowRowG = highRowG = lowColG = highColG = -1;
            YBrush = Brushes.LightGray;
            GRangeValues = "";

            mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

            if (SingleModeRadio)
            {
                PossibleFigures = true;
                IndivEmpirical = true;
                windowRef.groupKSetting1.IsEnabled = false;
                windowRef.groupKSetting2.IsEnabled = false;
                windowRef.groupKSetting3.IsEnabled = false;
                windowRef.gRangeButton.IsEnabled = false;
                windowRef.gRange.IsEnabled = false;
            }
            else if (BatchModeRadio)
            {
                PossibleFigures = false;
                GroupEmpirical = true;
                windowRef.groupKSetting1.IsEnabled = true;
                windowRef.groupKSetting2.IsEnabled = true;
                windowRef.groupKSetting3.IsEnabled = true;
                windowRef.gRangeButton.IsEnabled = true;
                windowRef.gRange.IsEnabled = true;
            }
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
            if (HurshModel)
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
        private void UpdateQ0Selectors()
        {
            if (FitQ0)
            {
                windowRef.qFixedValue.IsEnabled = false;
                //FixedQ0Value = "";

                windowRef.QRangeLow.IsEnabled = false;
                //BoundQ0ValueLow = "";
                windowRef.QRangeHigh.IsEnabled = false;
                //BoundQ0ValueHigh = "";
            }
            else if (FixQ0)
            {
                windowRef.qFixedValue.IsEnabled = true;
                //FixedQ0Value = "";

                windowRef.QRangeLow.IsEnabled = false;
                //BoundQ0ValueLow = "";
                windowRef.QRangeHigh.IsEnabled = false;
                //BoundQ0ValueHigh = "";
            }
            else if (BoundQ0)
            {
                windowRef.qFixedValue.IsEnabled = false;
                //FixedQ0Value = "";

                windowRef.QRangeLow.IsEnabled = true;
                //BoundQ0ValueLow = "";
                windowRef.QRangeHigh.IsEnabled = true;
                //BoundQ0ValueHigh = "";
            }
        }

        /// <summary>
        /// Update interface
        /// </summary>
        private void UpdateKSelectors()
        {
            if (CustomK)
            {
                windowRef.kRange.IsEnabled = true;
                windowRef.fitKNormal.IsEnabled = false;
                windowRef.fitKBounded.IsEnabled = false;
                windowRef.KRangeLow.IsEnabled = false;
                windowRef.KRangeHigh.IsEnabled = false;
            }
            else if (GroupFitted || indivFitted)
            {
                windowRef.KRangeLow.IsEnabled = true;
                windowRef.KRangeHigh.IsEnabled = true;
                windowRef.fitKNormal.IsEnabled = true;
                windowRef.fitKBounded.IsEnabled = true;
                windowRef.kRange.IsEnabled = false;
            }
            else
            {
                if (loaded)
                {
                    windowRef.kRange.IsEnabled = false;
                    windowRef.KRangeLow.IsEnabled = false;
                    windowRef.KRangeHigh.IsEnabled = false;
                    windowRef.fitKNormal.IsEnabled = false;
                    windowRef.fitKBounded.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Update interface
        /// </summary>
        private void UpdateKBoundingSelectors()
        {
            if (FitK)
            {
                windowRef.KRangeLow.IsEnabled = false;
                windowRef.KRangeHigh.IsEnabled = false;
            }
            else
            {
                if (loaded)
                {
                    windowRef.KRangeLow.IsEnabled = true;
                    windowRef.KRangeHigh.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// Query user for a range
        /// </summary>
        private void ResetKRange()
        {
            //KBrush = Brushes.LightGray;
            KRangeValues = "";

            lowColK = -1;
            highColK = -1;

            lowRowK = -1;
            highRowK = -1;
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
                        //KBrush = Brushes.LightSalmon;
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
                    if (SingleModeRadio)
                    {
                        if (RowModeRadio)
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
                                MessageBox.Show("Please ensure that only a single series is selected");
                            }
                        }
                        else if (ColumnModeRadio)
                        {
                            if ((DataGridTools.GetColumnIndex(secondChars) - DataGridTools.GetColumnIndex(firstChars)) == 0)
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
                                MessageBox.Show("Please ensure that only a single column is selected");
                            }
                        }
                    }
                    else if (BatchModeRadio)
                    {
                        if (RowModeRadio)
                        {
                            if ((sNum - fNum) > 1)
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
                                MessageBox.Show("Please ensure that more than one series is selected");
                            }
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

                SingleModeRadio = true;
                HurshModel = true;
                IndivEmpirical = true;
                FitQ0 = true;

                windowRef.singleCalculationButton.IsChecked = true;
                loaded = true;
                UpdateYSelectors();
                UpdateKSelectors();
                FitK = true;
                UpdateKBoundingSelectors();
                UpdateQ0Selectors();
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
                //HurshModel = true;
                //modelArraySelection = "Exponential";
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

            if (KRangeValues.Length < 1 || KRangeValues.ToLower().Contains("spreadsheet"))
            {
                KRangeValues = string.Empty;
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
        private void GetKRange()
        {
            mWindow.dataGrid.CommitEdit();

            DefaultFieldsToGray();

            //KBrush = Brushes.Yellow;
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

            if (cells.Count < 1 || itemSource.Count < 1) return;

            lowRowK = cells.Min(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));
            highRowK = cells.Max(i => DataGridTools.GetIndexViewModel((RowViewModel)i.Item, itemSource));

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

            //KBrush = Brushes.LightSalmon;
            KRangeValues = DataGridTools.GetColumnName(lowColK) + lowRowK.ToString() + ":" + DataGridTools.GetColumnName(highColK) + highRowK.ToString();

            KValue = "";
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

            if (!singleModeRadio && !batchModeRadio)
            {
                MessageBox.Show("Please select a mode of calculation.");
                return;
            }

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

            if (singleModeRadio)
            {
                if (RowModeRadio)
                {
                    if ((highColY - lowColY) < 2 || (highRowY - lowRowY) > 0)
                    {
                        DefaultFieldsToGray();

                        mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

                        lowColY = -1;
                        lowRowY = -1;
                        highColY = -1;
                        highRowY = -1;
                        MessageBox.Show("Please select a matrix of consumption values, with one row of values with at least three individual points of data (i.e., 1 x ... ).");

                        return;
                    }
                }
                else if (ColumnModeRadio)
                {
                    if ((highRowY - lowRowY) < 2 || (highColY - lowColY) > 0)
                    {
                        DefaultFieldsToGray();

                        mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

                        lowColY = -1;
                        lowRowY = -1;
                        highColY = -1;
                        highRowY = -1;
                        MessageBox.Show("Please select a matrix of consumption values, with one column of values with at least three individual points of data (i.e., 1 x ... ).");

                        return;
                    }
                }

            }
            else if (batchModeRadio)
            {
                if (RowModeRadio)
                {
                    if ((highColY - lowColY) < 2 || (highRowY - lowRowY) < 1)
                    {
                        DefaultFieldsToGray();

                        mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

                        lowColY = -1;
                        lowRowY = -1;
                        highColY = -1;
                        highRowY = -1;
                        MessageBox.Show("Please select a matrix of consumption values, with more than one row of values with at least three individual points of data (i.e., 3x3).");

                        return;
                    }
                }
                else if (ColumnModeRadio)
                {
                    if ((highRowY - lowRowY) < 2 || (highColY - lowColY) < 1)
                    {
                        DefaultFieldsToGray();

                        mWindow.dataGrid.PreviewMouseUp -= DataGrid_PreviewMouseUp_Y;

                        lowColY = -1;
                        lowRowY = -1;
                        highColY = -1;
                        highRowY = -1;
                        MessageBox.Show("Please select a matrix of consumption values, with more than one column of values with at least three individual points of data (i.e., 3x3).");

                        return;
                    }
                }
            }
            else
            {
                return;
            }

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
        /// Routing command for mode-specific scoring
        /// </summary>
        /// <returns></returns>
        private void PreScoring()
        {
            if (singleModeRadio)
            {
                CalculateScores();
            }
            else if (batchModeRadio)
            {
                CalculateBatchScores();
            }
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

            double derivedK = -1;

            mWindow.OutputEvents(" ");
            mWindow.OutputEvents("---------------------------------------------------");

            mWindow.OutputEvents("Checking user-supplied ranges and reference points....");

            List<double>[] array = null;

            if (RowModeRadio)
            {
                array = DataGridTools.GetRangedValuesHorizontal(lowColX, highColX, lowRowX, lowColY, highColY, lowRowY, mWindow.dataGrid.ItemsSource);
            }
            else if (ColumnModeRadio)
            {
                array = DataGridTools.GetRangedValuesVertical(lowRowX, highRowX, lowColX, lowRowY, highRowY, lowColY, mWindow.dataGrid.ItemsSource);
            }

            List<double> xRange = new List<double>(array[0]);
            List<double> yRange = new List<double>(array[1]);

            double lowY = yRange.Where(v => v > 0).OrderBy(v => v).First();
            double highY = yRange.Where(v => v > 0).OrderBy(v => v).Last();

            kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;

            mWindow.OutputEvents("Data passed null and type checks...");
            mWindow.OutputEvents("Determining a fitting heuristic...");

            YValueDecisions yBehavior = YValueDecisions.DropZeros;
            XValueDecisions xBehavior;
            KValueDecisions kBehavior = KValueDecisions.DeriveValuesIndividual;

            #region AdvancedMenuCustomizations

            if (advancedMenu)
            {
                xBehavior = (DropXValues) ? XValueDecisions.DropZeros : XValueDecisions.DoNothing;

                // Y Value
                if (DropYValues)
                {
                    yBehavior = YValueDecisions.DropZeros;
                }
                else if (KeepYValues)
                {
                    yBehavior = YValueDecisions.DoNothing;
                }
                else if (ModYValuesHundredth)
                {
                    yBehavior = YValueDecisions.ChangeHundredth;
                }
                else if (ModYValueTenth)
                {
                    yBehavior = YValueDecisions.ChangeTenth;
                }
                else if (ModYValues)
                {
                    yBehavior = YValueDecisions.ChangeCustom;

                    if (!double.TryParse(YModValues, out yModValue))
                    {
                        mWindow.OutputEvents("Your modified Y value does not appear valid.");
                        MessageBox.Show("Your supplied Y value does not appear correct.");
                        return;
                    }
                }

                // K Value
                if (IndivFitted)
                {
                    kBehavior = KValueDecisions.FitK;
                }
                else if (IndivEmpirical)
                {
                    kBehavior = KValueDecisions.DeriveValuesIndividual;

                    kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;
                }
                else if (CustomK)
                {
                    kBehavior = KValueDecisions.UseSuppliedValues;

                    if (!double.TryParse(KValue, out kValueDouble))
                    {
                        mWindow.OutputEvents("Your supplied K value does not appear correct.");
                        MessageBox.Show("Your supplied K value does not appear correct.");
                        return;
                    }
                }
            }
            else
            {
                engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

                yBehavior = Decisions.GetYBehavior(HurshModel, windowRef);

                if (yBehavior == YValueDecisions.Cancel)
                {
                    return;
                }

                xBehavior = Decisions.GetXBehavior(HurshModel, windowRef);

                if (xBehavior == XValueDecisions.Cancel)
                {
                    return;
                }

                kBehavior = Decisions.GetKBehaviorIndividual(windowRef);

                if (kBehavior == KValueDecisions.Cancel)
                {
                    return;
                }

                if (kBehavior == KValueDecisions.DeriveValuesIndividual)
                {
                    kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;
                }
                else if (kBehavior == KValueDecisions.FitK)
                {
                    IndivFitted = true;
                }
                else if (kBehavior == KValueDecisions.UseSuppliedValues)
                {
                    var mKprompt = new RangePrompt();
                    mKprompt.Topmost = true;
                    mKprompt.Owner = windowRef;
                    mKprompt.questionText.Text = "Enter K value:";
                    mKprompt.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                    if (mKprompt.ShowDialog() == true)
                    {
                        KValue = mKprompt.ResponseText;
                    }

                    if (!double.TryParse(KValue, out kValueDouble))
                    {
                        mWindow.OutputEvents("Your supplied K value does not appear correct.");
                        MessageBox.Show("Your supplied K value does not appear correct.");
                        return;
                    }
                }
            }

            #endregion

            #region DataAdjustments

            mWindow.OutputEvents("---------------------------------------------------");

            List<double> xTemp = new List<double>();
            List<double> yTemp = new List<double>();
            List<double> pTemp = new List<double>();

            List<DemandCoordinate> demandPoints = new List<DemandCoordinate>();

            for (int j = 0; j < xRange.Count; j++)
            {
                yTemp.Add(yRange[j]);
                xTemp.Add(xRange[j]);
                pTemp.Add(1);

                demandPoints.Add(new DemandCoordinate
                {
                    X = xRange[j],
                    Y = yRange[j],
                    P = 1,
                    Expend = (xRange[j] * yRange[j])
                });
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
                else if (yBehavior == YValueDecisions.ChangeTenth && yTemp[0] == 0)
                {
                    yTemp[i] = 0.1;
                }
                else if (yBehavior == YValueDecisions.ChangeCustom && yTemp[0] == 0)
                {
                    yTemp[i] = yModValue;
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

            #region KBoundChecks

            double boundLowKtemp = double.NaN;
            double boundHighKtemp = double.NaN;

            if (BoundK && !(double.TryParse(BoundKValueLow, out boundLowKtemp) && double.TryParse(BoundKValueHigh, out boundHighKtemp)))
            {
                mWindow.OutputEvents("Your supplied K bounds are not valid.");
                MessageBox.Show("Your supplied K bounds are not valid.");
                return;
            }
            else if (boundLowKtemp >= boundHighKtemp)
            {
                mWindow.OutputEvents("Your supplied K bounds seem reversed?");
                MessageBox.Show("Your supplied K bounds seem reversed?");
                return;
            }

            #endregion

            #region Q0BoundChecks

            double boundLowQtemp = double.NaN;
            double boundHighQtemp = double.NaN;
            double fixedQ0temp = double.NaN;

            if (FixQ0 && !double.TryParse(FixedQ0Value, out fixedQ0temp))
            {
                mWindow.OutputEvents("Your supplied Q0 is not valid.");
                MessageBox.Show("Your supplied Q0 is not valid.");
                return;
            }
            else if (fixedQ0temp <= 0)
            {
                mWindow.OutputEvents("Your supplied Q0 must be greater than 0.");
                MessageBox.Show("Your supplied Q0 must be greater than 0.");
                return;
            }

            if (BoundQ0 && !(double.TryParse(BoundQ0ValueHigh, out boundHighQtemp) && double.TryParse(BoundQ0ValueLow, out boundLowQtemp)))
            {
                mWindow.OutputEvents("Your supplied Q0 bounds are not valid.");
                MessageBox.Show("Your supplied Q0 bounds are not valid.");
                return;
            }
            else if (boundLowQtemp >= boundHighQtemp)
            {
                mWindow.OutputEvents("Your supplied Q0 bounds seem reversed?");
                MessageBox.Show("Your supplied Q0 bounds seem reversed?");
                return;
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

            winHack.ShowDialog();

            if (winHack.MessageOptions.SelectedValue.ToString().Contains("review") || !winHack.GotClicked)
            {
                return;
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

            for (int i = 0; i < yRange.Count + 10; i++)
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

                mVM.RowViewModels[0].values[19] = "TotalPass";
                mVM.RowViewModels[0].values[20] = "DeltaQ";
                mVM.RowViewModels[0].values[21] = "DeltaQPass";
                mVM.RowViewModels[0].values[22] = "Bounce";
                mVM.RowViewModels[0].values[23] = "BouncePass";
                mVM.RowViewModels[0].values[24] = "Reversals";
                mVM.RowViewModels[0].values[25] = "ReversalsPass";
                mVM.RowViewModels[0].values[26] = "NumPosValues";
                mVM.RowViewModels[0].values[27] = "Notes";

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

                yValues = engine.CreateNumericVector(yRange.ToArray());
                engine.SetSymbol("yLoad", yValues);

                xValues = engine.CreateNumericVector(xRange.ToArray());
                engine.SetSymbol("xLoad", xValues);

                /// A clone of p essentially, since single
                NumericVector gValues = engine.CreateNumericVector(pRange.ToArray());
                engine.SetSymbol("gLoad", gValues);

                //engine.Evaluate("gLoad <- rep(1," + kRange.Count.ToString() + ")");

                if (FixQ0)
                {
                    engine.Evaluate("FixedQ0 <- " + FixedQ0Value.ToString());
                }
                else
                {
                    engine.Evaluate("FixedQ0 <- NULL");
                }

                if (BoundQ0)
                {
                    engine.Evaluate("minQ0 <- " + BoundQ0ValueLow.ToString());
                    engine.Evaluate("maxQ0 <- " + BoundQ0ValueHigh.ToString());
                }
                else
                {
                    engine.Evaluate("minQ0 <- 0.01");
                    engine.Evaluate("maxQ0 <- Inf");
                }

                if (BoundK)
                {
                    engine.Evaluate("maxK <- " + boundKValueHigh.ToString());
                    engine.Evaluate("minK <- " + boundKValueLow.ToString());
                }
                else
                {
                    engine.Evaluate("maxK <- " + yValues.Max().ToString());
                    engine.Evaluate("minK <- 0.1");
                }


                if (HurshModel)
                {
                    if (kBehavior == KValueDecisions.FitK)
                    {
                        if (IndivFitted)
                        {
                            engine.Evaluate("fittingFlag <- 2");
                        }
                        else if (GroupFitted)
                        {
                            engine.Evaluate("fittingFlag <- 1");
                        }

                        engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunctionKFittings());
                    }
                    else
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunction());
                    }

                }
                else if (KoffarnusModel)
                {
                    if (kBehavior == KValueDecisions.FitK)
                    {
                        if (IndivFitted)
                        {
                            engine.Evaluate("fittingFlag <- 2");
                        }
                        else if (GroupFitted)
                        {
                            engine.Evaluate("fittingFlag <- 1");
                        }

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

                    mVM.RowViewModels[1].values[16] = engine.Evaluate("fitFrame[fitFrame$p==1,]$EV").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[17] = engine.Evaluate("fitFrame[fitFrame$p==1,]$OmaxD").AsVector().First().ToString();
                    mVM.RowViewModels[1].values[18] = engine.Evaluate("fitFrame[fitFrame$p==1,]$PmaxD").AsVector().First().ToString();

                }
                else
                {
                    for (int i = 2; i <= 18; i++)
                    {
                        mVM.RowViewModels[1].values[i] = "NA";
                    }

                    mVM.RowViewModels[1].values[21] = "Model did not converge, was a curve actually present?";
                }

                mVM.RowViewModels[1].values[11] = DemandFunctionSolvers.GetOmaxE(demandPoints);
                mVM.RowViewModels[1].values[12] = DemandFunctionSolvers.GetPmaxE(demandPoints);
                mVM.RowViewModels[1].values[13] = DemandFunctionSolvers.GetQ0E(demandPoints);
                mVM.RowViewModels[1].values[14] = DemandFunctionSolvers.GetBP0(demandPoints);
                mVM.RowViewModels[1].values[15] = DemandFunctionSolvers.GetBP1(demandPoints);

                mVM.RowViewModels[1].values[19] = results[0, "TotalPass"].ToString();
                mVM.RowViewModels[1].values[20] = results[0, "DeltaQ"].ToString();
                mVM.RowViewModels[1].values[21] = results[0, "DeltaQPass"].ToString();
                mVM.RowViewModels[1].values[22] = results[0, "Bounce"].ToString();
                mVM.RowViewModels[1].values[23] = results[0, "BouncePass"].ToString();
                mVM.RowViewModels[1].values[24] = results[0, "Reversals"].ToString();
                mVM.RowViewModels[1].values[25] = results[0, "ReversalsPass"].ToString();
                mVM.RowViewModels[1].values[26] = results[0, "NumPosValues"].ToString();

                string modelSelected = (HurshModel) ? "Exponential" : "Exponentiated";

                mVM.RowViewModels[5].values[0] = "Model: " + modelSelected;
                mVM.RowViewModels[6].values[0] = "Analysis : " + "Individual";
                mVM.RowViewModels[7].values[0] = "Y Behavior: " + Decisions.GetYBehaviorDescription(yBehavior);

                if (ModYValuesCustom)
                {
                    mVM.RowViewModels[7].values[0] = mVM.RowViewModels[7].values[0] + " (" + yModValue + ")";
                }

                mVM.RowViewModels[8].values[0] = "X Behavior: " + Decisions.GetXBehaviorDescription(xBehavior);
                mVM.RowViewModels[9].values[0] = "K Behavior: " + Decisions.GetKBehaviorDescription(kBehavior);

                mWin.Owner = mWindow;
                mWin.Show();

                if (outputFigures)
                {
                    xRange = new List<double>(array[0]);
                    yRange = new List<double>(array[1]);

                    engine.Evaluate("SourceFrame<-data.frame(x=c(" + string.Join(",", xRange) + ")," +
                        "y=c(" + string.Join(",", yRange) + "))");
                    engine.Evaluate("SourceFrame$p <- 1");

                    try
                    {
                        engine.Evaluate("library(ggplot2)");
                        engine.Evaluate("library(reshape2)");
                        engine.Evaluate("library(gridExtra)");

                        engine.Evaluate("textOmax <- '\n Empirical oMax: " + mVM.RowViewModels[1].values[11] +
                            "\n Derived oMax: " + String.Format("{0:0.##}", double.Parse(mVM.RowViewModels[1].values[17])) + "'");
                        engine.Evaluate("textPmax <- '\n Empirical pMax: " + mVM.RowViewModels[1].values[12] +
                            "\n Derived pMax: " + String.Format("{0:0.##}", double.Parse(mVM.RowViewModels[1].values[18])) + "'");

                        engine.Evaluate("graphingOmax <- fitFrame[fitFrame$p==1,]$OmaxD");
                        engine.Evaluate("graphingPmax <- fitFrame[fitFrame$p==1,]$PmaxD");

                        engine.Evaluate("empP <- " + DemandFunctionSolvers.GetPmaxE(demandPoints));
                        engine.Evaluate("derP <- " + engine.Evaluate("fitFrame[fitFrame$p==1,]$PmaxD").AsVector().First().ToString());

                        if (HurshModel)
                        {
                            if (xRange.Contains(0))
                            {
                                engine.Evaluate(DemandFunctionSolvers.GetExponentialGraphingFunctionFaceted());
                            }
                            else
                            {
                                engine.Evaluate(DemandFunctionSolvers.GetExponentialGraphingFunction());
                            }
                        }
                        else if (KoffarnusModel)
                        {
                            if (xRange.Contains(0))
                            {
                                engine.Evaluate(DemandFunctionSolvers.GetExponentiatedGraphingFunctionFaceted());
                            }
                            else
                            {
                                engine.Evaluate(DemandFunctionSolvers.GetExponentiatedGraphingFunction());
                            }
                        }

                        WpfDrawingSettings settings = new WpfDrawingSettings();
                        settings.IncludeRuntime = true;
                        settings.TextAsGeometry = false;

                        string output = engine.Evaluate("demandString").AsVector().First().ToString();

                        byte[] bytes = Convert.FromBase64String(output);

                        path1 = Path.GetTempFileName();

                        if (File.Exists(path1))
                        {
                            File.Delete(path1);
                        }

                        File.WriteAllBytes(path1, bytes);

                        FileSvgReader converter1 = new FileSvgReader(settings);
                        DrawingGroup drawing1 = converter1.Read(path1);

                        if (drawing1 != null)
                        {
                            var iWindow1 = new ImageWindow();
                            iWindow1.filePath = path1;
                            iWindow1.imageHolder.Source = new DrawingImage(drawing1);
                            iWindow1.Show();
                        }

                        /*

                        string output2 = engine.Evaluate("workString").AsVector().First().ToString();

                        byte[] bytes2 = Convert.FromBase64String(output2);
                        path2 = Path.GetTempFileName();

                        if (File.Exists(path2))
                        {
                            File.Delete(path2);
                        }

                        File.WriteAllBytes(path2, bytes2);

                        FileSvgReader converter2 = new FileSvgReader(settings);
                        DrawingGroup drawing2 = converter2.Read(path2);

                        if (drawing2 != null)
                        {
                            var iWindow2 = new ImageWindow();
                            iWindow2.filePath = path2;
                            iWindow2.imageHolder.Source = new DrawingImage(drawing2);
                            iWindow2.Show();
                        }

                        */

                        filesList.Add(path1);
                        //filesList.Add(path2);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }

                mWindow.OutputEvents("Please remember to cite the packages used in this process!");
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation()$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('ggplot2')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('gridExtra')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('nlmrt')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('nlstools')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('base64enc')$textVersion").AsCharacter().ToArray()));
                mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('reshape2')$textVersion").AsCharacter().ToArray()));

            }
            catch (ParseException pe)
            {
                Console.WriteLine(pe.ToString());
            }

            mWindow.dataGrid.IsReadOnly = false;
            #endregion

        }

        /// <summary>
        /// Command-call to calculate based on supplied ranges and reference values (max value).
        /// Will reference user-selected options (figures, outputs, etc.) throughout calls to R
        /// </summary>
        private void CalculateBatchScores()
        {
            mWindow.dataGrid.CommitEdit();

            if (failed) return;

            if (lowColX < 0 || lowColY < 0) return;

            double derivedK = -1;

            mWindow.OutputEvents(" ");
            mWindow.OutputEvents("---------------------------------------------------");

            mWindow.OutputEvents("Checking user-supplied ranges and reference points....");

            List<double> xRange = null;
            List<double> gRange = null;
            string[,] wholeRange = null;

            if (RowModeRadio)
            {
                xRange = DataGridTools.GetRangedValuesVM(lowColX, highColX, lowRowX, mWindow.dataGrid.ItemsSource);
                wholeRange = DataGridTools.ParseBulkRangeStringsVM(lowRowY, highRowY, lowColY, highColY, mWindow.dataGrid.ItemsSource);

                if (lowRowG != -1)
                {
                    gRange = DataGridTools.GetRangedValuesVerticalVM(lowRowG, highRowG, lowColG, mWindow.dataGrid.ItemsSource);
                }
            }
            else if (ColumnModeRadio)
            {
                xRange = DataGridTools.GetRangedValuesVerticalVM(lowRowX, highRowX, lowColX, mWindow.dataGrid.ItemsSource);
                wholeRange = DataGridTools.ParseBulkRangeStringsVerticalVM(lowRowY, highRowY, lowColY, highColY, mWindow.dataGrid.ItemsSource);

                if (lowRowG != -1)
                {
                    gRange = DataGridTools.GetRangedValuesVM(lowColG, highColG, lowRowG, mWindow.dataGrid.ItemsSource);
                }
            }

            if (xRange == null)
            {
                mWindow.OutputEvents("Error while validating the Pricing values.  There cannot be any blank, null or non-numeric fields.");
                MessageBox.Show("Please review the Pricing values.  There cannot be any blank, null or non-numeric fields.");
                return;
            }

            if (gRange == null && lowRowG != -1)
            {
                mWindow.OutputEvents("Error while validating the Grouping values.  There cannot be any blank or null fields.");
                MessageBox.Show("Please review the Grouping values.  There cannot be any blank or null fields.");
                return;
            }
            else if (gRange != null)
            {
                if (gRange.Count != wholeRange.GetLength(1))
                {
                    mWindow.OutputEvents("Error while preparing the Grouping values.  Elements must be provided for all series.");
                    MessageBox.Show("Error while preparing the Grouping values.  Elements must be provided for all series.");
                    return;
                }
            }

            if (wholeRange == null)
            {
                mWindow.OutputEvents("There were items that failed validation in the Consumption values.  Are any fields blank or not numeric?");
                MessageBox.Show("There were items that failed validation in the Consumption values.");
                return;
            }

            List<double> kRanges = null;

            YValueDecisions yBehavior = YValueDecisions.DropZeros;
            XValueDecisions xBehavior;
            KValueDecisions kBehavior = KValueDecisions.DeriveValuesGroup;

            mWindow.OutputEvents("Data passed null and type checks...");
            mWindow.OutputEvents("Determining a fitting heuristic...");

            List<double> valueRange = new List<double>();

            for (int i = 0; i < wholeRange.GetLength(0); i++)
            {
                for (int j = 0; j < wholeRange.GetLength(1); j++)
                {
                    valueRange.Add(double.Parse(wholeRange[i, j]));
                }
            }

            double lowY = valueRange.Where(v => v > 0).OrderBy(v => v).First();
            double highY = valueRange.Where(v => v > 0).OrderBy(v => v).Last();

            mWindow.OutputEvents("Applying settings...");

            #region AdvancedMenuCustomizations

            if (AdvancedMenu)
            {
                kRanges = DataGridTools.GetRangedValuesVerticalVM(lowRowK, highRowK, lowColK, mWindow.dataGrid.ItemsSource);

                if (kRanges != null)
                {
                    if (kRanges.Count() > 1 && kRanges.Count() != wholeRange.GetLength(1))
                    {
                        mWindow.OutputEvents("Your custom k ranges don't match the # of rows.");
                        MessageBox.Show("Hmm, check your k range.  It doesn't seem paired up with the rows.");
                        return;
                    }
                }
                else if (kRanges == null && lowRowK != -1)
                {
                    mWindow.OutputEvents("There were items that failed validation in the K values.  Are any fields blank or not numeric?");
                    MessageBox.Show("There were items that failed validation in the K values.");
                    return;
                }

                xBehavior = (DropXValues) ? XValueDecisions.DropZeros : XValueDecisions.DoNothing;

                if (DropYValues)
                {
                    yBehavior = YValueDecisions.DropZeros;
                }
                else if (KeepYValues)
                {
                    yBehavior = YValueDecisions.DoNothing;
                }
                else if (ModYValueTenth)
                {
                    yBehavior = YValueDecisions.ChangeTenth;
                }
                else if (ModYValuesHundredth)
                {
                    yBehavior = YValueDecisions.ChangeHundredth;
                }
                else if (ModYValuesCustom)
                {
                    yBehavior = YValueDecisions.ChangeCustom;
                }

                if (GroupEmpirical)
                {
                    kBehavior = KValueDecisions.DeriveValuesGroup;
                }
                else if (GroupEmpiricalAverage)
                {
                    kValueAverage = DemandFunctionSolvers.GetAverageLogK(wholeRange);
                    kBehavior = KValueDecisions.AverageLogValuesGroup;
                }
                else if (GroupFitted || IndivFitted)
                {
                    kBehavior = KValueDecisions.FitK;
                }
                else if (IndivEmpirical)
                {
                    kBehavior = KValueDecisions.DeriveValuesIndividual;
                    kValueDouble = (Math.Log10(highY) - Math.Log10(lowY)) + 0.5;
                }
                else if (CustomK)
                {
                    kBehavior = KValueDecisions.UseSuppliedValues;

                    if (!double.TryParse(KValue, out kValueDouble))
                    {
                        mWindow.OutputEvents("Your supplied K value does not appear correct.");
                        MessageBox.Show("Your supplied K value does not appear correct.");
                        return;
                    }
                }
            }
            else
            {
                yBehavior = Decisions.GetYBehavior(HurshModel, windowRef);

                if (yBehavior == YValueDecisions.Cancel)
                {
                    return;
                }

                xBehavior = Decisions.GetXBehavior(HurshModel, windowRef);

                if (xBehavior == XValueDecisions.Cancel)
                {
                    return;
                }

                kBehavior = Decisions.GetKBehaviorGroup(windowRef);

                if (kBehavior == KValueDecisions.Cancel)
                {
                    return;
                }
            }

            #endregion

            mWindow.OutputEvents("Applying heuristics...");

            #region FittingHeuristic

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

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

            double[] lowestAndHighest = DataGridTools.GetLowestAndHighestInMatrix(wholeRange);

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
                else if (yBehavior == YValueDecisions.ChangeTenth && yTemp[0] == 0)
                {
                    yTemp[i] = 0.1;
                }
                else if (yBehavior == YValueDecisions.OnePercentLowest && yTemp[0] == 0)
                {
                    yTemp[i] = lowestAndHighest[0] / 100;
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

            mWindow.OutputEvents("Applying bounds and start values...");

            #region KBoundChecks

            double boundLowKtemp = double.NaN;
            double boundHighKtemp = double.NaN;

            if (BoundK && !(double.TryParse(BoundKValueLow, out boundLowKtemp) && double.TryParse(BoundKValueHigh, out boundHighKtemp)))
            {
                mWindow.OutputEvents("Your supplied K bounds are not valid.");
                MessageBox.Show("Your supplied K bounds are not valid.");
                return;
            }
            else if (boundLowKtemp >= boundHighKtemp)
            {
                mWindow.OutputEvents("Your supplied K bounds seem reversed?");
                MessageBox.Show("Your supplied K bounds seem reversed?");
                return;
            }

            #endregion

            #region Q0BoundChecks

            double boundLowQtemp = double.NaN;
            double boundHighQtemp = double.NaN;
            double fixedQ0temp = double.NaN;

            if (FixQ0 && !double.TryParse(FixedQ0Value, out fixedQ0temp))
            {
                mWindow.OutputEvents("Your supplied Q0 is not valid.");
                MessageBox.Show("Your supplied Q0 is not valid.");
                return;
            }
            else if (fixedQ0temp <= 0)
            {
                mWindow.OutputEvents("Your supplied Q0 must be greater than 0.");
                MessageBox.Show("Your supplied Q0 must be greater than 0.");
                return;
            }

            if (BoundQ0 && !(double.TryParse(BoundQ0ValueHigh, out boundHighQtemp) && double.TryParse(BoundQ0ValueLow, out boundLowQtemp)))
            {
                mWindow.OutputEvents("Your supplied Q0 bounds are not valid.");
                MessageBox.Show("Your supplied Q0 bounds are not valid.");
                return;
            }
            else if (boundLowQtemp >= boundHighQtemp)
            {
                mWindow.OutputEvents("Your supplied Q0 bounds seem reversed?");
                MessageBox.Show("Your supplied Q0 bounds seem reversed?");
                return;
            }

            #endregion

            mWindow.OutputEvents("Beginning Stein's Tests...");

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

            winHack.ShowDialog();

            if (winHack.MessageOptions.SelectedValue.ToString().Contains("review") || !winHack.GotClicked)
            {
                return;
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

            mWindow.OutputEvents("All inputs passed verification.");
            mWindow.OutputEvents("---------------------------------------------------");
            mWindow.OutputEvents("Beginning Batched Computations...");

            var mWin = new ResultsWindow();
            var mVM = new ResultsViewModel();
            mWin.DataContext = mVM;

            for (int i = 0; i < wholeRange.GetLength(1) + 20; i++)
            {
                mVM.RowViewModels.Add(new RowViewModel());
            }

            double[] yLowHigh = DataGridTools.GetLowestAndHighestInMatrix(wholeRange);

            derivedK = (Math.Log10(yLowHigh[1]) - Math.Log10(yLowHigh[0])) + 0.5;

            #endregion

            #region GroupFitting

            List<DemandCoordinate> demandPoints = new List<DemandCoordinate>();

            engine.Evaluate("rm(list = setdiff(ls(), lsf.str()))");

            yRange = new List<double>();
            xRangeShadow = new List<double>();
            List<double> kRange = new List<double>();
            List<double> pRange = new List<double>();
            List<double> groupRange = new List<double>();

            if (yBehavior == YValueDecisions.ChangeCustom && !AdvancedMenu)
            {
                var mYprompt = new RangePrompt();
                mYprompt.Topmost = true;
                mYprompt.Owner = windowRef;
                mYprompt.questionText.Text = "Modified Y value:";
                mYprompt.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (mYprompt.ShowDialog() == true)
                {
                    YModValues = mYprompt.ResponseText;
                }

                if (!double.TryParse(YModValues, out yModValue))
                {
                    mWindow.OutputEvents("Your modified Y value does not appear valid.");
                    MessageBox.Show("Your supplied Y value does not appear correct.");
                    return;
                }
            }

            if (kBehavior == KValueDecisions.UseSuppliedValues && !AdvancedMenu)
            {
                var mKprompt = new RangePrompt();
                mKprompt.Topmost = true;
                mKprompt.Owner = windowRef;
                mKprompt.questionText.Text = "Enter K value:";
                mKprompt.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (mKprompt.ShowDialog() == true)
                {
                    KValue = mKprompt.ResponseText;
                }

                if (!double.TryParse(KValue, out kValueDouble))
                {
                    mWindow.OutputEvents("Your supplied K value does not appear correct.");
                    MessageBox.Show("Your supplied K value does not appear correct.");
                    return;
                }
            }

            mWindow.OutputEvents("Beginning low-level analyses...");

            try
            {

                #region SequencedKCalculations

                for (int mIndex = 0; mIndex < wholeRange.GetLength(1); mIndex++)
                {
                    demandPoints = new List<DemandCoordinate>();


                    for (int i = 0; i < wholeRange.GetLength(0); i++)
                    {
                        if (double.TryParse(wholeRange[i, mIndex], out holder))
                        {
                            yRange.Add(holder);
                            xRangeShadow.Add(xRange[i]);
                            pRange.Add(mIndex + 1);

                            if (gRange == null)
                            {
                                groupRange.Add(1);
                            }
                            else
                            {
                                groupRange.Add(gRange[mIndex]);
                            }

                            demandPoints.Add(new DemandCoordinate
                            {
                                X = xRange[i],
                                Y = holder,
                                P = 1 + mIndex,
                                Expend = (xRange[i] * holder)
                            });

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
                            else if (kBehavior == KValueDecisions.AverageLogValuesGroup)
                            {
                                kValueAverage = DemandFunctionSolvers.GetAverageLogK(wholeRange);
                                kRange.Add(kValueAverage);
                            }
                            else if (kBehavior == KValueDecisions.UseSuppliedValues)
                            {
                                kRange.Add(kValueDouble);
                            }
                        }
                    }

                }

                #endregion

                #region DataAdjustments

                if (yBehavior == YValueDecisions.DoNothing)
                {
                    // Nothing different
                }
                else if (yBehavior == YValueDecisions.ChangeCustom)
                {
                    List<double> yCopy = new List<double>();

                    foreach (double y in yRange)
                    {
                        if (y == 0)
                        {
                            yCopy.Add(yModValue);
                        }
                        else
                        {
                            yCopy.Add(y);
                        }
                    }

                    yRange = new List<double>(yCopy);
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
                else if (yBehavior == YValueDecisions.ChangeTenth)
                {
                    List<double> yCopy = new List<double>();

                    foreach (double y in yRange)
                    {
                        if (y == 0)
                        {
                            yCopy.Add(0.1);
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
                List<double> gTemp = new List<double>(groupRange);
                List<double> kTemp = new List<double>(kRange);

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
                        gTemp.RemoveAt(index);

                        if (kBehavior != KValueDecisions.FitK)
                        {
                            kTemp.RemoveAt(index);
                        }
                    }

                    yRange = new List<double>(yTemp);
                    xRangeShadow = new List<double>(xTemp);
                    pRange = new List<double>(pTemp);
                    groupRange = new List<double>(gTemp);

                    if (kBehavior != KValueDecisions.FitK)
                    {
                        kRange = new List<double>(kTemp);
                    }
                }

                #endregion

                NumericVector kValues = engine.CreateNumericVector(kRange.ToArray());
                engine.SetSymbol("kLoad", kValues);

                NumericVector participantValues = engine.CreateNumericVector(pRange.ToArray());
                engine.SetSymbol("pLoad", participantValues);

                NumericVector yValues = engine.CreateNumericVector(yRange.ToArray());
                engine.SetSymbol("yLoad", yValues);

                NumericVector xValues = engine.CreateNumericVector(xRangeShadow.ToArray());
                engine.SetSymbol("xLoad", xValues);

                NumericVector gValues = engine.CreateNumericVector(groupRange.ToArray());
                engine.SetSymbol("gLoad", gValues);

                if (FixQ0)
                {
                    engine.Evaluate("FixedQ0 <- " + FixedQ0Value.ToString());
                }
                else
                {
                    engine.Evaluate("FixedQ0 <- NULL");
                }

                if (BoundQ0)
                {
                    engine.Evaluate("minQ0 <- " + BoundQ0ValueLow.ToString());
                    engine.Evaluate("maxQ0 <- " + BoundQ0ValueHigh.ToString());
                }
                else
                {
                    engine.Evaluate("minQ0 <- 0.01");
                    engine.Evaluate("maxQ0 <- Inf");
                }

                if (BoundK)
                {
                    engine.Evaluate("maxK <- " + boundKValueHigh.ToString());
                    engine.Evaluate("minK <- " + boundKValueLow.ToString());
                }
                else
                {
                    if (heldMaxY == double.NaN)
                    {
                        heldMaxY = yValues.Max();
                    }
                    else if (heldMaxY < yValues.Max())
                    {
                        heldMaxY = yValues.Max();
                    }
                    
                    engine.Evaluate("maxK <- " + yValues.Max().ToString());
                    engine.Evaluate("minK <- 0.1");
                }

                if (HurshModel)
                {
                    if (kBehavior == KValueDecisions.FitK)
                    {
                        if (IndivFitted)
                        {
                            engine.Evaluate("fittingFlag <- 2");
                        }
                        else
                        {
                            engine.Evaluate("fittingFlag <- 1");
                        }

                        engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunctionKFittings());
                    }
                    else
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentialDemandFunctionKSet());
                    }
                }
                else if (KoffarnusModel)
                {
                    if (kBehavior == KValueDecisions.FitK)
                    {
                        if (IndivFitted)
                        {
                            engine.Evaluate("fittingFlag <- 2");
                        }
                        else if (GroupFitted)
                        {
                            engine.Evaluate("fittingFlag <- 1");
                        }

                        engine.Evaluate(DemandFunctionSolvers.GetExponentiatedDemandFunctionKFittings());
                    }
                    else
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetExponentiatedDemandFunctionKSet());
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
                mVM.RowViewModels[0].values[19] = "TotalPass";
                mVM.RowViewModels[0].values[20] = "DeltaQ";
                mVM.RowViewModels[0].values[21] = "DeltaQPass";
                mVM.RowViewModels[0].values[22] = "Bounce";
                mVM.RowViewModels[0].values[23] = "BouncePass";
                mVM.RowViewModels[0].values[24] = "Reversals";
                mVM.RowViewModels[0].values[25] = "ReversalsPass";
                mVM.RowViewModels[0].values[26] = "NumPosValues";
                mVM.RowViewModels[0].values[27] = "Notes";

                if (kBehavior == KValueDecisions.FitK)
                {
                    #region FittedGroupedK

                    var groupedSeriesFrame = engine.Evaluate("fitFrame").AsDataFrame();

                    // NA's default to true in R.Net
                    if (groupedSeriesFrame[0, "q0"].ToString() != "True")
                    {
                        mVM.RowViewModels[1].values[0] = "Aggregated Fitting";

                        if (kBehavior == KValueDecisions.FitK)
                        {
                            mVM.RowViewModels[1].values[1] = groupedSeriesFrame[0, "k"].ToString();
                            double.TryParse(groupedSeriesFrame[0, "k"].ToString(), out heldK);
                        }
                        else
                        {
                            mVM.RowViewModels[1].values[1] = kRange.Min().ToString();
                            heldK = kRange.Min();
                        }

                        mVM.RowViewModels[1].values[2] = groupedSeriesFrame[0, "q0"].ToString();
                        mVM.RowViewModels[1].values[3] = groupedSeriesFrame[0, "alpha"].ToString();
                        mVM.RowViewModels[1].values[4] = groupedSeriesFrame[0, "q0err"].ToString();
                        mVM.RowViewModels[1].values[5] = groupedSeriesFrame[0, "alphaerr"].ToString();

                        string qLow = groupedSeriesFrame[0, "q0low"].ToString();
                        string qHigh = groupedSeriesFrame[0, "q0high"].ToString();
                        string aLow = groupedSeriesFrame[0, "alow"].ToString();
                        string aHigh = groupedSeriesFrame[0, "ahigh"].ToString();

                        mVM.RowViewModels[1].values[6] = qLow + " - " + qHigh;
                        mVM.RowViewModels[1].values[7] = aLow + " - " + aHigh;

                        mVM.RowViewModels[1].values[8] = groupedSeriesFrame[0, "r2"].ToString();
                        mVM.RowViewModels[1].values[9] = groupedSeriesFrame[0, "absSS"].ToString();
                        mVM.RowViewModels[1].values[10] = groupedSeriesFrame[0, "sdResid"].ToString();
                        mVM.RowViewModels[1].values[11] = "---";
                        mVM.RowViewModels[1].values[12] = "---";
                        mVM.RowViewModels[1].values[13] = "---";
                        mVM.RowViewModels[1].values[14] = "---";
                        mVM.RowViewModels[1].values[15] = "---";
                        /*
                        mVM.RowViewModels[1].values[11] = DemandFunctionSolvers.GetOmaxEGroup(xRange, wholeRange);
                        mVM.RowViewModels[1].values[12] = DemandFunctionSolvers.GetPmaxEGroup(xRange, wholeRange);
                        mVM.RowViewModels[1].values[13] = DemandFunctionSolvers.GetQ0EGroup(xRange, wholeRange);
                        mVM.RowViewModels[1].values[14] = DemandFunctionSolvers.GetBP0Group(xRange, wholeRange);
                        mVM.RowViewModels[1].values[15] = DemandFunctionSolvers.GetBP1Group(xRange, wholeRange);
                        */
                        mVM.RowViewModels[1].values[16] = groupedSeriesFrame[0, "EV"].ToString();
                        mVM.RowViewModels[1].values[17] = groupedSeriesFrame[0, "OmaxD"].ToString();
                        mVM.RowViewModels[1].values[18] = groupedSeriesFrame[0, "PmaxD"].ToString();

                        for (int i = 0; i <= 18; i++)
                        {
                            mVM.RowViewModels[1].values[i] = mVM.RowViewModels[1].values[i].Replace("True", "NA");
                        }

                    }
                    else
                    {
                        for (int i = 2; i <= 18; i++)
                        {
                            mVM.RowViewModels[1].values[i] = "NA";
                        }

                        mVM.RowViewModels[1].values[29] = "Model did not converge, was a curve actually present?";
                    }

                    int rowNumber = 3;
                    int rowBuffer = 0;

                    if (gRange != null)
                    {
                        rowBuffer += gRange.GroupBy(x => x).Select(y => y.First()).Count() + 1;
                    }

                    var individualSeriesFrame = engine.Evaluate("fitFrameTemp").AsDataFrame();

                    foreach (var row in individualSeriesFrame.GetRows())
                    {
                        if (row["q0"].ToString() != "True")
                        {
                            mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Series #" + row["p"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[1] = row["k"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[2] = row["q0"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[3] = row["alpha"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[4] = row["q0err"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[5] = row["alphaerr"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[6] = row["q0low"].ToString() + " - " + row["q0high"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[7] = row["alow"].ToString() + " - " + row["ahigh"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[8] = row["r2"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[9] = row["absSS"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[10] = row["sdResid"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[11] = DemandFunctionSolvers.GetOmaxEGroup(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[12] = DemandFunctionSolvers.GetPmaxEGroup(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[13] = DemandFunctionSolvers.GetQ0EGroup(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[14] = DemandFunctionSolvers.GetBP0Group(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[15] = DemandFunctionSolvers.GetBP1Group(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[16] = row["EV"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[17] = row["OmaxD"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[18] = row["PmaxD"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[19] = results[rowNumber - 3, "TotalPass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[20] = results[rowNumber - 3, "DeltaQ"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[21] = results[rowNumber - 3, "DeltaQPass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[22] = results[rowNumber - 3, "Bounce"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[23] = results[rowNumber - 3, "BouncePass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[24] = results[rowNumber - 3, "Reversals"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[25] = results[rowNumber - 3, "ReversalsPass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[26] = results[rowNumber - 3, "NumPosValues"].ToString();

                            for (int i = 0; i <= 26; i++)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[i] = mVM.RowViewModels[rowNumber + rowBuffer].values[i].Replace("True", "NA");
                            }

                        }
                        else
                        {
                            for (int i = 2; i <= 18; i++)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[i] = "NA";
                            }

                            mVM.RowViewModels[rowNumber + rowBuffer].values[27] = "Model did not converge, was a curve actually present?";
                        }

                        rowNumber++;

                    }

                    string modelSelected = (HurshModel) ? "Exponential" : "Exponentiated";
                    string calculationSelected = (SingleModeRadio) ? "Single" : "Grouped";

                    rowNumber += 2;

                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Model: " + modelSelected;
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Analysis : " + calculationSelected;
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Y Behavior: " + Decisions.GetYBehaviorDescription(yBehavior);
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "X Behavior: " + Decisions.GetXBehaviorDescription(xBehavior);
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "K Behavior: " + Decisions.GetKBehaviorDescription(kBehavior);
                    rowNumber++;


                    for (int i = 0; i < 15; i++)
                    {
                        mVM.RowViewModels.Add(new RowViewModel());
                    }

                    rowNumber++;

                    bool isGrouping = engine.Evaluate("isGrouped").AsLogical().First();

                    if (isGrouping)
                    {
                        bool isAnova = engine.Evaluate("isAnova").AsLogical().First();
                        bool isParamGood = engine.Evaluate("!is.null(output)").AsLogical().First();
                        bool isNonParamGood = engine.Evaluate("!is.null(outputNP)").AsLogical().First();

                        if (!isAnova)
                        {

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Method";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$method").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[6] = "Method";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = engine.Evaluate("outputNP$method").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "T:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$statistic").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[6] = "W:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = engine.Evaluate("outputNP$statistic").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "df:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$parameter").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[6] = "p:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = engine.Evaluate("outputNP$p.value").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "p:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$p.value").AsVector().First().ToString();
                                rowNumber++;
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "95% CI";

                                double loCI = engine.Evaluate("output$conf.int[1]").AsNumeric().FirstOrDefault();
                                double hiCI = engine.Evaluate("output$conf.int[2]").AsNumeric().FirstOrDefault();
                                string ciRange = loCI.ToString("0.0000") + "-" + hiCI.ToString("0.0000");

                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = ciRange;
                                rowNumber++;
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Mean 1";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$estimate[1]").AsVector().First().ToString();
                                rowNumber++;
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Mean 2";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$estimate[2]").AsVector().First().ToString();
                                rowNumber++;
                                rowNumber++;
                            }

                        }
                        else
                        {
                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Method: One-way Anova";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = "Method: Kruskal-Wall RST";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "group:";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = "alpha by group:";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "residuals:";
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = "df";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[8] = "chi-squared";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output[[1]][['Df']][1]").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[8] = engine.Evaluate("outputNP$statistic").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output[[1]][['Df']][2]").AsVector().First().ToString();
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[2] = "Sum Sq";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[9] = "df";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[2] = engine.Evaluate("output[[1]][['Sum Sq']][1]").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[9] = engine.Evaluate("outputNP$parameter").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[2] = engine.Evaluate("output[[1]][['Sum Sq']][2]").AsVector().First().ToString();
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[3] = "Mean Sq";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[10] = "p value";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[3] = engine.Evaluate("output[[1]][['Mean Sq']][1]").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[10] = engine.Evaluate("outputNP$p.value").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[3] = engine.Evaluate("output[[1]][['Mean Sq']][2]").AsVector().First().ToString();
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[4] = "F value";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[4] = engine.Evaluate("output[[1]][['F value']][1]").AsVector().First().ToString();
                            }

                            rowNumber++;
                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[5] = "Pr(>F)";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[5] = engine.Evaluate("output[[1]][['Pr(>F)']][1]").AsVector().First().ToString();
                            }

                            rowNumber++;
                            rowNumber++;
                            rowNumber++;

                        }
                    }

                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "X";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Mean";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "SD";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "% Zero";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "% NA";
                    rowNumber++;
                    
                    List<double> currentValues;
                    for (int j = 0; j < wholeRange.GetLength(0); j++)
                    {
                        rowNumber -= 5;

                        currentValues = new List<double>();

                        for (int i = 0; i < wholeRange.GetLength(1); i++)
                        {
                            currentValues.Add(Double.Parse(wholeRange[j, i]));
                        }

                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = xRange[j].ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = currentValues.Average().ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = DataGridTools.StandardDeviation(currentValues).ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = ((double)((double)currentValues.Count(v => v == 0) / (double)currentValues.Count()) * 100).ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = "";
                        rowNumber++;

                    }

                    #endregion
                }
                else
                {
                    #region GroupStuff

                    var groupedSeriesFrame = engine.Evaluate("fitFrame").AsDataFrame();

                    // NA's default to true in R.Net
                    if (groupedSeriesFrame[0, "q0"].ToString() != "True")
                    {
                        mVM.RowViewModels[1].values[0] = "Aggregated Fitting";

                        if (kBehavior == KValueDecisions.FitK)
                        {
                            mVM.RowViewModels[1].values[1] = groupedSeriesFrame[0, "k"].ToString();
                            double.TryParse(groupedSeriesFrame[0, "k"].ToString(), out heldK);
                        }
                        else
                        {
                            mVM.RowViewModels[1].values[1] = kRange.Min().ToString();
                            heldK = kRange.Min();
                        }

                        mVM.RowViewModels[1].values[2] = groupedSeriesFrame[0, "q0"].ToString();
                        mVM.RowViewModels[1].values[3] = groupedSeriesFrame[0, "alpha"].ToString();
                        mVM.RowViewModels[1].values[4] = groupedSeriesFrame[0, "q0err"].ToString();
                        mVM.RowViewModels[1].values[5] = groupedSeriesFrame[0, "alphaerr"].ToString();

                        string qLow = groupedSeriesFrame[0, "q0low"].ToString();
                        string qHigh = groupedSeriesFrame[0, "q0high"].ToString();
                        string aLow = groupedSeriesFrame[0, "alow"].ToString();
                        string aHigh = groupedSeriesFrame[0, "ahigh"].ToString();

                        mVM.RowViewModels[1].values[6] = qLow + " - " + qHigh;
                        mVM.RowViewModels[1].values[7] = aLow + " - " + aHigh;
                        mVM.RowViewModels[1].values[8] = groupedSeriesFrame[0, "r2"].ToString();
                        mVM.RowViewModels[1].values[9] = groupedSeriesFrame[0, "absSS"].ToString();
                        mVM.RowViewModels[1].values[10] = groupedSeriesFrame[0, "sdResid"].ToString();
                        mVM.RowViewModels[1].values[11] = "---";
                        mVM.RowViewModels[1].values[12] = "---";
                        mVM.RowViewModels[1].values[13] = "---";
                        mVM.RowViewModels[1].values[14] = "---";
                        mVM.RowViewModels[1].values[15] = "---";
                        /*
                        mVM.RowViewModels[1].values[11] = DemandFunctionSolvers.GetOmaxEGroup(xRange, wholeRange);
                        mVM.RowViewModels[1].values[12] = DemandFunctionSolvers.GetPmaxEGroup(xRange, wholeRange);
                        mVM.RowViewModels[1].values[13] = DemandFunctionSolvers.GetQ0EGroup(xRange, wholeRange);
                        mVM.RowViewModels[1].values[14] = DemandFunctionSolvers.GetBP0Group(xRange, wholeRange);
                        mVM.RowViewModels[1].values[15] = DemandFunctionSolvers.GetBP1Group(xRange, wholeRange);
                        */
                        mVM.RowViewModels[1].values[16] = groupedSeriesFrame[0, "EV"].ToString();
                        mVM.RowViewModels[1].values[17] = groupedSeriesFrame[0, "OmaxD"].ToString();
                        mVM.RowViewModels[1].values[18] = groupedSeriesFrame[0, "PmaxD"].ToString();

                        for (int i = 0; i <= 18; i++)
                        {
                            mVM.RowViewModels[1].values[i] = mVM.RowViewModels[1].values[i].Replace("True", "NA");
                        }
                    }
                    else
                    {
                        for (int i = 2; i <= 18; i++)
                        {
                            mVM.RowViewModels[1].values[i] = "NA";
                        }

                        mVM.RowViewModels[1].values[19] = "Model did not converge, was a curve actually present?";
                    }

                    int rowNumber = 3;
                    int rowBuffer = 0;

                    if (gRange != null)
                    {
                        rowBuffer += gRange.GroupBy(x => x).Select(y => y.First()).Count() + 1;
                    }

                    var individualSeriesFrame = engine.Evaluate("fitFrameTemp").AsDataFrame();

                    foreach (var row in individualSeriesFrame.GetRows())
                    {
                        if (row["q0"].ToString() != "True")
                        {
                            mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Series #" + row["p"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[1] = row["k"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[2] = row["q0"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[3] = row["alpha"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[4] = row["q0err"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[5] = row["alphaerr"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[6] = row["q0low"].ToString() + " - " + row["q0high"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[7] = row["alow"].ToString() + " - " + row["ahigh"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[8] = row["r2"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[9] = row["absSS"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[10] = row["sdResid"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[11] = DemandFunctionSolvers.GetOmaxEGroup(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[12] = DemandFunctionSolvers.GetPmaxEGroup(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[13] = DemandFunctionSolvers.GetQ0EGroup(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[14] = DemandFunctionSolvers.GetBP0Group(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[15] = DemandFunctionSolvers.GetBP1Group(xRange, wholeRange, rowNumber - 3);
                            mVM.RowViewModels[rowNumber + rowBuffer].values[16] = row["EV"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[17] = row["OmaxD"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[18] = row["PmaxD"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[19] = results[rowNumber - 3, "TotalPass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[20] = results[rowNumber - 3, "DeltaQ"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[21] = results[rowNumber - 3, "DeltaQPass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[22] = results[rowNumber - 3, "Bounce"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[23] = results[rowNumber - 3, "BouncePass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[24] = results[rowNumber - 3, "Reversals"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[25] = results[rowNumber - 3, "ReversalsPass"].ToString();
                            mVM.RowViewModels[rowNumber + rowBuffer].values[26] = results[rowNumber - 3, "NumPosValues"].ToString();

                            for (int i = 0; i <= 26; i++)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[i] = mVM.RowViewModels[rowNumber + rowBuffer].values[i].Replace("True", "NA");
                            }

                        }
                        else
                        {
                            for (int i = 2; i <= 18; i++)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[i] = "NA";
                            }

                            mVM.RowViewModels[rowNumber + rowBuffer].values[27] = "Model did not converge, was a curve actually present?";
                        }

                        rowNumber++;

                    }

                    string modelSelected = (HurshModel) ? "Exponential" : "Exponentiated";
                    string calculationSelected = (SingleModeRadio) ? "Single" : "Grouped";

                    rowNumber+=2;

                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Model: " + modelSelected;
                        rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Analysis : " + calculationSelected;
                        rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Y Behavior: " + Decisions.GetYBehaviorDescription(yBehavior);
                        rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "X Behavior: " + Decisions.GetXBehaviorDescription(xBehavior);
                        rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "K Behavior: " + Decisions.GetKBehaviorDescription(kBehavior);
                        rowNumber++;


                    for (int i = 0; i < 15; i++)
                    {
                        mVM.RowViewModels.Add(new RowViewModel());
                    }

                    rowNumber++;

                    bool isGrouping = engine.Evaluate("isGrouped").AsLogical().First();

                    if (isGrouping)
                    {
                        bool isAnova = engine.Evaluate("isAnova").AsLogical().First();
                        bool isParamGood = engine.Evaluate("!is.null(output)").AsLogical().First();
                        bool isNonParamGood = engine.Evaluate("!is.null(outputNP)").AsLogical().First();

                        if (!isAnova)
                        {

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Method";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$method").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[6] = "Method";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = engine.Evaluate("outputNP$method").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "T:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$statistic").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[6] = "W:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = engine.Evaluate("outputNP$statistic").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "df:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$parameter").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[6] = "p:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = engine.Evaluate("outputNP$p.value").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "p:";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$p.value").AsVector().First().ToString();
                                rowNumber++;
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "95% CI";

                                double loCI = engine.Evaluate("output$conf.int[1]").AsNumeric().FirstOrDefault();
                                double hiCI = engine.Evaluate("output$conf.int[2]").AsNumeric().FirstOrDefault();
                                string ciRange = loCI.ToString("0.0000") + "-" + hiCI.ToString("0.0000");

                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = ciRange;
                                rowNumber++;
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Mean 1";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$estimate[1]").AsVector().First().ToString();
                                rowNumber++;
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Mean 2";
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output$estimate[2]").AsVector().First().ToString();
                                rowNumber++;
                                rowNumber++;
                            }

                        }
                        else
                        {
                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Method: One-way Anova";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = "Method: Kruskal-Wall RST";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "group:";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[7] = "alpha by group:";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "residuals:";
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = "df";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[8] = "chi-squared";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output[[1]][['Df']][1]").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[8] = engine.Evaluate("outputNP$statistic").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[1] = engine.Evaluate("output[[1]][['Df']][2]").AsVector().First().ToString();
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[2] = "Sum Sq";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[9] = "df";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[2] = engine.Evaluate("output[[1]][['Sum Sq']][1]").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[9] = engine.Evaluate("outputNP$parameter").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[2] = engine.Evaluate("output[[1]][['Sum Sq']][2]").AsVector().First().ToString();
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[3] = "Mean Sq";
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[10] = "p value";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[3] = engine.Evaluate("output[[1]][['Mean Sq']][1]").AsVector().First().ToString();
                            }

                            if (isNonParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[10] = engine.Evaluate("outputNP$p.value").AsVector().First().ToString();
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[3] = engine.Evaluate("output[[1]][['Mean Sq']][2]").AsVector().First().ToString();
                            }

                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[4] = "F value";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[4] = engine.Evaluate("output[[1]][['F value']][1]").AsVector().First().ToString();
                            }

                            rowNumber++;
                            rowNumber++;

                            rowNumber -= 3;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[5] = "Pr(>F)";
                            }

                            rowNumber++;

                            if (isParamGood)
                            {
                                mVM.RowViewModels[rowNumber + rowBuffer].values[5] = engine.Evaluate("output[[1]][['Pr(>F)']][1]").AsVector().First().ToString();
                            }

                            rowNumber++;
                            rowNumber++;
                            rowNumber++;

                        }
                    }


                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "X";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "Mean";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "SD";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "% Zero";
                    rowNumber++;
                    mVM.RowViewModels[rowNumber + rowBuffer].values[0] = "% NA";
                    rowNumber++;


                    List<double> currentValues;
                    for (int j = 0; j < wholeRange.GetLength(0); j++)
                    {
                        rowNumber -= 5;

                        currentValues = new List<double>();

                        for (int i = 0; i < wholeRange.GetLength(1); i++)
                        {
                            currentValues.Add(Double.Parse(wholeRange[j, i]));
                        }

                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = xRange[j].ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = currentValues.Average().ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = DataGridTools.StandardDeviation(currentValues).ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = ((double)((double)currentValues.Count(v => v == 0) / (double)currentValues.Count()) * 100).ToString();
                        rowNumber++;
                        mVM.RowViewModels[rowNumber + rowBuffer].values[j + 1] = "";
                        rowNumber++;

                    }

                    #endregion 
                }

            }
            catch (ParseException pe)
            {
                Console.WriteLine(pe.ToString());
            }

            #endregion

            mWindow.OutputEvents("Beginning high-level analyses...");

            try
            {
                if (FixQ0)
                {
                    engine.Evaluate("FixedQ0 <- " + FixedQ0Value.ToString());
                }
                else
                {
                    engine.Evaluate("FixedQ0 <- NULL");
                }

                engine.Evaluate("StaticK <- " + heldK.ToString());

                if (BoundQ0)
                {
                    engine.Evaluate("minQ0 <- " + BoundQ0ValueLow.ToString());
                    engine.Evaluate("maxQ0 <- " + BoundQ0ValueHigh.ToString());
                }
                else
                {
                    engine.Evaluate("minQ0 <- 0.01");
                    engine.Evaluate("maxQ0 <- Inf");
                }

                if (BoundK)
                {
                    engine.Evaluate("maxK <- " + boundKValueHigh.ToString());
                    engine.Evaluate("minK <- " + boundKValueLow.ToString());
                }
                else
                {
                    engine.Evaluate("maxK <- " + heldMaxY.ToString());
                    engine.Evaluate("minK <- 0.1");
                }

                if (OutputFigures)
                {
                    engine.Evaluate("actuallyChart <- TRUE");
                }
                else
                {
                    engine.Evaluate("actuallyChart <- FALSE");
                }

                if (HurshModel)
                {
                    if (xRange.Contains(0) && xBehavior == XValueDecisions.DropZeros)
                    {
                        List<double> xRangeG = new List<double>(xRange);

                        int index = xRangeG.FindIndex(v => v == 0);

                        engine.Evaluate("nRowsToAdd <- " + wholeRange.GetLength(1));
                        engine.Evaluate("newRows <- data.frame(p=seq(1,nRowsToAdd,1),y=rep(NA,nRowsToAdd),x=rep(NA,nRowsToAdd),k=rep(NA,nRowsToAdd),g=rep(NA,nRowsToAdd))");

                        for (int participant = 0; participant < wholeRange.GetLength(1); participant++)
                        {
                            double p = participant + 1;
                            double y = double.Parse(wholeRange[index, participant]);
                            double g = gRange[participant];
                            double k = 1;

                            string dynamicEval = string.Format("newRows[newRows$p=={0},]$x <- {1}", p, 0);
                            engine.Evaluate(dynamicEval);

                            dynamicEval = string.Format("newRows[newRows$p=={0},]$y <- {1}", p, y);
                            engine.Evaluate(dynamicEval);

                            dynamicEval = string.Format("newRows[newRows$p=={0},]$g <- {1}", p, g);
                            engine.Evaluate(dynamicEval);

                            dynamicEval = string.Format("newRows[newRows$p=={0},]$k <- {1}", p, k);
                            engine.Evaluate(dynamicEval);
                        }

                        if (kBehavior == KValueDecisions.FitK)
                        {
                            engine.Evaluate("newRows <- subset(newRows, select= -c(k))");
                        }

                        engine.Evaluate("SourceFrame <- rbind(SourceFrame, newRows)");

                        engine.Evaluate(DemandFunctionSolvers.GetAggregateExponentialGraphingFaceted());
                    }
                    else if (xRange.Contains(0) && xBehavior != XValueDecisions.DropZeros)
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetAggregateExponentialGraphingFaceted());
                    }
                    else
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetAggregateExponentialGraphing());
                    }
                }
                else
                {
                    if (xRange.Contains(0) && xBehavior == XValueDecisions.DropZeros)
                    {
                        List<double> xRangeG = new List<double>(xRange);

                        int index = xRangeG.FindIndex(v => v == 0);

                        engine.Evaluate("nRowsToAdd <- " + wholeRange.GetLength(1));
                        engine.Evaluate("newRows <- data.frame(p=seq(1,nRowsToAdd,1),y=rep(NA,nRowsToAdd),x=rep(NA,nRowsToAdd),k=rep(NA,nRowsToAdd),g=rep(NA,nRowsToAdd))");

                        for (int participant = 0; participant < wholeRange.GetLength(1); participant++)
                        {
                            double p = participant + 1;
                            double y = double.Parse(wholeRange[index, participant]);
                            double g = gRange[participant];
                            double k = 1;

                            string dynamicEval = string.Format("newRows[newRows$p=={0},]$x <- {1}", p, 0);
                            engine.Evaluate(dynamicEval);

                            dynamicEval = string.Format("newRows[newRows$p=={0},]$y <- {1}", p, y);
                            engine.Evaluate(dynamicEval);

                            dynamicEval = string.Format("newRows[newRows$p=={0},]$g <- {1}", p, g);
                            engine.Evaluate(dynamicEval);

                            dynamicEval = string.Format("newRows[newRows$p=={0},]$k <- {1}", p, k);
                            engine.Evaluate(dynamicEval);
                        }

                        if (kBehavior == KValueDecisions.FitK)
                        {
                            engine.Evaluate("newRows <- subset(newRows, select= -c(k))");
                        }

                        engine.Evaluate("SourceFrame <- rbind(SourceFrame, newRows)");

                        engine.Evaluate(DemandFunctionSolvers.GetAggregateExponentialGraphingFaceted());
                    }
                    else if (xRange.Contains(0) && xBehavior != XValueDecisions.DropZeros)
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetAggregateExponentialGraphingFaceted());
                    }
                    else
                    {
                        engine.Evaluate(DemandFunctionSolvers.GetAggregateExponentiatedGraphing());
                    }
                }

                var groupedSeriesFrame = engine.Evaluate("fitFrameTemp").AsDataFrame();

                int rowNumber = 3;

                foreach (var row in groupedSeriesFrame.GetRows())
                {
                    if (row["q0"].ToString() != "True")
                    {
                        mVM.RowViewModels[rowNumber].values[0] = "Group #" + row["g"].ToString();
                        mVM.RowViewModels[rowNumber].values[1] = row["k"].ToString();
                        mVM.RowViewModels[rowNumber].values[2] = row["q0"].ToString();
                        mVM.RowViewModels[rowNumber].values[3] = row["alpha"].ToString();
                        mVM.RowViewModels[rowNumber].values[4] = row["q0err"].ToString();
                        mVM.RowViewModels[rowNumber].values[5] = row["alphaerr"].ToString();
                        mVM.RowViewModels[rowNumber].values[6] = row["q0low"].ToString() + " - " + row["q0high"].ToString();
                        mVM.RowViewModels[rowNumber].values[7] = row["alow"].ToString() + " - " + row["ahigh"].ToString();
                        mVM.RowViewModels[rowNumber].values[8] = row["r2"].ToString();
                        mVM.RowViewModels[rowNumber].values[9] = row["absSS"].ToString();
                        mVM.RowViewModels[rowNumber].values[10] = row["sdResid"].ToString();
                        mVM.RowViewModels[rowNumber].values[11] = "---";
                        mVM.RowViewModels[rowNumber].values[12] = "---";
                        mVM.RowViewModels[rowNumber].values[13] = "---";
                        mVM.RowViewModels[rowNumber].values[14] = "---";
                        mVM.RowViewModels[rowNumber].values[15] = "---";
                        /*
                        mVM.RowViewModels[rowNumber].values[11] = DemandFunctionSolvers.GetOmaxEGroup(xRange, wholeRange, rowNumber - 3);
                        mVM.RowViewModels[rowNumber].values[12] = DemandFunctionSolvers.GetPmaxEGroup(xRange, wholeRange, rowNumber - 3);
                        mVM.RowViewModels[rowNumber].values[13] = DemandFunctionSolvers.GetQ0EGroup(xRange, wholeRange, rowNumber - 3);
                        mVM.RowViewModels[rowNumber].values[14] = DemandFunctionSolvers.GetBP0Group(xRange, wholeRange, rowNumber - 3);
                        mVM.RowViewModels[rowNumber].values[15] = DemandFunctionSolvers.GetBP1Group(xRange, wholeRange, rowNumber - 3);
                        */
                        mVM.RowViewModels[rowNumber].values[16] = row["EV"].ToString();
                        mVM.RowViewModels[rowNumber].values[17] = row["OmaxD"].ToString();
                        mVM.RowViewModels[rowNumber].values[18] = row["PmaxD"].ToString();
                        mVM.RowViewModels[rowNumber].values[19] = "NA";
                        mVM.RowViewModels[rowNumber].values[20] = "NA";
                        mVM.RowViewModels[rowNumber].values[21] = "NA";
                        mVM.RowViewModels[rowNumber].values[22] = "NA";
                        mVM.RowViewModels[rowNumber].values[23] = "NA";
                        mVM.RowViewModels[rowNumber].values[24] = "NA";
                        mVM.RowViewModels[rowNumber].values[25] = "NA";
                        mVM.RowViewModels[rowNumber].values[26] = "NA";

                        for (int i=0; i<=26; i++)
                        {
                            mVM.RowViewModels[rowNumber].values[i] = mVM.RowViewModels[rowNumber].values[i].Replace("True", "NA");
                        }

                    }
                    else
                    {
                        for (int i = 2; i <= 18; i++)
                        {
                            mVM.RowViewModels[rowNumber].values[i] = "NA";
                        }

                        mVM.RowViewModels[rowNumber].values[27] = "Model did not converge, was a curve actually present?";
                    }

                    rowNumber++;

                }

                if (OutputFigures)
                {
                    mWindow.OutputEvents("Constructing figures...");

                    WpfDrawingSettings settings = new WpfDrawingSettings();
                    settings.IncludeRuntime = true;
                    settings.TextAsGeometry = false;

                    string output = engine.Evaluate("demandString").AsVector().First().ToString();

                    byte[] bytes = Convert.FromBase64String(output);

                    path1 = Path.GetTempFileName();

                    if (File.Exists(path1))
                    {
                        File.Delete(path1);
                    }

                    File.WriteAllBytes(path1, bytes);

                    FileSvgReader converter1 = new FileSvgReader(settings);
                    DrawingGroup drawing1 = converter1.Read(path1);

                    if (drawing1 != null)
                    {
                        var iWindow1 = new ImageWindow();
                        iWindow1.filePath = path1;
                        iWindow1.imageHolder.Source = new DrawingImage(drawing1);
                        iWindow1.Show();
                    }

                    filesList.Add(path1);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            mWindow.OutputEvents("Calculations complete!");

            mWindow.OutputEvents("Please remember to cite the packages used in this process!");
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation()$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('ggplot2')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('gridExtra')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('nlmrt')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('nlstools')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('base64enc')$textVersion").AsCharacter().ToArray()));
            mWindow.OutputEvents("Citation:: " + string.Join("", engine.Evaluate("citation('reshape2')$textVersion").AsCharacter().ToArray()));

            mWin.Owner = mWindow;
            mWin.Show();
        }
    }
}
