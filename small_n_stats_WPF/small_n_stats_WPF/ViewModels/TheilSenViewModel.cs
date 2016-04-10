﻿/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * Small n Stats Application
 * Modeled from conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using small_n_stats_WPF.Graphics;
using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Models;
using small_n_stats_WPF.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using small_n_stats_WPF.Utilities;
using unvell.ReoGrid;

namespace small_n_stats_WPF.ViewModels
{
    class TheilSenViewModel : ViewModelBase
    {
        internal MainWindowViewModel mInterface;
        internal MainWindow mWindow;
        internal TheilSenWindow windowRef;

        /* Color Binds*/

        private Brush baselineBackGround = Brushes.White;
        public Brush BaselineBackGround
        {
            get { return baselineBackGround; }
            set
            {
                baselineBackGround = value;
                OnPropertyChanged("BaselineBackGround");
            }
        }

        private Brush interventionBackGround = Brushes.White;
        public Brush InterventionBackGround
        {
            get { return interventionBackGround; }
            set
            {
                interventionBackGround = value;
                OnPropertyChanged("InterventionBackGround");
            }
        }

        /* String binds */

        private string baselineRangeString = "";
        public string BaselineRangeString
        {
            get { return baselineRangeString; }
            set
            {
                baselineRangeString = value;
                OnPropertyChanged("BaselineRangeString");
            }
        }

        private string interventionRangeString = "";
        public string InterventionRangeString
        {
            get { return interventionRangeString; }
            set
            {
                interventionRangeString = value;
                OnPropertyChanged("InterventionRangeString");
            }
        }

        /* Bool flags */

        private bool chartFlag = false;
        public bool ChartFlag
        {
            get { return chartFlag; }
            set
            {
                chartFlag = value;
                OnPropertyChanged("ChartFlag");
            }
        }

        /* Command Logic */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand LoadBaselineDataCommand { get; set; }
        public RelayCommand LoadInterventionDataCommand { get; set; }
        public RelayCommand CalculateCommand { get; set; }

        /* Math */

        private Sen mSenObj;

        public TheilSenViewModel()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            LoadBaselineDataCommand = new RelayCommand(param => LoadBaselineData(), param => true);
            LoadInterventionDataCommand = new RelayCommand(param => LoadInterventionData(), param => true);
            CalculateCommand = new RelayCommand(param => Calculate(), param => true);
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        private void ViewLoaded()
        {
            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Loading Theil-Sen modules...");

            mSenObj = new Sen();

            mInterface.SendMessageToOutput("Sen modules loaded.");
        }

        private void DefaultFieldsToGray()
        {
            if (BaselineRangeString.Length < 1 || BaselineRangeString.ToLower().Contains("sheet"))
            {
                BaselineBackGround = Brushes.LightGray;
                BaselineRangeString = string.Empty;
            }

            if (InterventionRangeString.Length < 1 || InterventionRangeString.ToLower().Contains("sheet"))
            {
                InterventionBackGround = Brushes.LightGray;
                InterventionRangeString = string.Empty;
            }
        }

        private void LoadBaselineData()
        {
            DefaultFieldsToGray();

            if (BaselineRangeString.Length > 0 && !BaselineRangeString.ToLower().Contains("spreadsheet"))
            {
                /* Restore past ranges to white */
                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(mWindow.spreadSheetView.CurrentWorksheet.Ranges[BaselineRangeString], new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = Colors.Transparent,
                });
            }

            BaselineBackGround = Brushes.Yellow;
            BaselineRangeString = "Select baseline data on sheet";

            mWindow.spreadSheetView.PickRange((inst, range) =>
            {
                if (range.Cols > 1)
                {
                    DefaultFieldsToGray();
                    MessageBox.Show("Please select a single column of data");
                    return true;
                }

                BaselineBackGround = Brushes.LightBlue;
                BaselineRangeString = range.ToString();

                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(range, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = Colors.LightBlue,
                });

                return true;

            }, Cursors.Cross);

        }

        private void LoadInterventionData()
        {
            DefaultFieldsToGray();

            if (InterventionRangeString.Length > 0 && !InterventionRangeString.ToLower().Contains("spreadsheet"))
            {
                /* Restore past ranges to white */
                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(mWindow.spreadSheetView.CurrentWorksheet.Ranges[InterventionRangeString], new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = Colors.Transparent,
                });
            }

            InterventionBackGround = Brushes.Yellow;
            InterventionRangeString = "Select intervention data on sheet";

            mWindow.spreadSheetView.PickRange((inst, range) =>
            {
                if (range.Cols > 1)
                {
                    DefaultFieldsToGray();
                    MessageBox.Show("Please select a single column of data");
                    return true;
                }

                InterventionBackGround = Brushes.LightGreen;
                InterventionRangeString = range.ToString();

                mWindow.spreadSheetView.CurrentWorksheet.SetRangeStyles(range, new WorksheetRangeStyle
                {
                    Flag = PlainStyleFlag.BackColor,
                    BackColor = Colors.LightGreen,
                });

                return true;

            }, Cursors.Cross);
        }

        private void Calculate()
        {
            if (BaselineRangeString.Length < 1 || InterventionRangeString.Length < 1)
            {
                MessageBox.Show("Please select ranges for both phases.");
                return;
            }

            List<double> blRange = mWindow.ParseRange(BaselineRangeString);
            List<double> txRange = mWindow.ParseRange(InterventionRangeString);

            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Calculating T-Sen Slopes: Baseline");

            SenSlopeModel bl = mSenObj.ComputeTheilSen(blRange);

            mInterface.SendMessageToOutput(System.String.Format("Baseline --- TS: {0},  SE-Slope: {1}, Z: {2}, P value: {3}", 
                bl.Sen.ToString("0.000"), 
                bl.SenSD.ToString("0.000"), 
                bl.Zscore.ToString("0.000"), 
                bl.P.ToString("0.000")));
            mInterface.SendMessageToOutput((System.String.Format("Baseline --- @ 85% CI: ({0} - {1})", 
                bl.TS85[0].ToString("0.000"), 
                bl.TS85[1].ToString("0.000"))));
            mInterface.SendMessageToOutput((System.String.Format("Baseline --- @ 90% CI: ({0} - {1})", 
                bl.TS90[0].ToString("0.000"), 
                bl.TS90[1].ToString("0.000"))));
            mInterface.SendMessageToOutput((System.String.Format("Baseline --- @ 95% CI: ({0} - {1})", 
                bl.TS95[0].ToString("0.000"), 
                bl.TS95[1].ToString("0.000"))));

            SenSlopeModel tx = mSenObj.ComputeTheilSen(txRange);

            mInterface.SendMessageToOutput(System.String.Format("Treatment --- TS: {0},  SE-Slope: {1}, Z: {2}, P value: {3}", 
                tx.Sen.ToString("0.000"), 
                tx.SenSD.ToString("0.000"), 
                tx.Zscore.ToString("0.000"), 
                tx.P.ToString("0.000")));
            mInterface.SendMessageToOutput((System.String.Format("Treatment --- @ 85% CI: ({0} - {1})", 
                tx.TS85[0].ToString("0.000"), 
                tx.TS85[1].ToString("0.000"))));
            mInterface.SendMessageToOutput((System.String.Format("Treatment --- @ 90% CI: ({0} - {1})", 
                tx.TS90[0].ToString("0.000"), 
                tx.TS90[1].ToString("0.000"))));
            mInterface.SendMessageToOutput((System.String.Format("Treatment --- @ 95% CI: ({0} - {1})", 
                tx.TS95[0].ToString("0.000"), 
                tx.TS95[1].ToString("0.000"))));

            SenSlopeModel comp = mSenObj.CompareTheilSen(bl, tx);

            mInterface.SendMessageToOutput(System.String.Format("Difference --- TS: {0},  SE-Slope: {1}, Z: {2}, P value: {3}", 
                comp.Sen.ToString("0.000"), 
                comp.SenSD.ToString("0.000"), 
                comp.Zscore.ToString("0.000"), 
                comp.P.ToString("0.000")));
            mInterface.SendMessageToOutput((System.String.Format("Difference --- @ 85% CI: ({0} - {1})", 
                comp.TS85[0].ToString("0.000"), 
                comp.TS85[1].ToString("0.000"))));
            mInterface.SendMessageToOutput((System.String.Format("Difference --- @ 90% CI: ({0} - {1})", 
                comp.TS90[0].ToString("0.000"), 
                comp.TS90[1].ToString("0.000"))));
            mInterface.SendMessageToOutput((System.String.Format("Difference --- @ 95% CI: ({0} - {1})", 
                comp.TS95[0].ToString("0.000"), 
                comp.TS95[1].ToString("0.000"))));

            SenSlopeModel newBl = mSenObj.ComputeTheilSen(new List<double>(blRange));
            SenSlopeModel blInterceptObj = mSenObj.GetInterceptBaseline(newBl);

            SenSlopeModel newTx = mSenObj.ComputeTheilSen(new List<double>(txRange));
            SenSlopeModel txInterceptObj = mSenObj.GetInterceptTreatment(newTx, newBl.Observations.Count);

            var chartWin = new ChartingWindow();
            Chart chart = chartWin.FindName("MyWinformChart") as Chart;
            chart.Series.Clear();

            // Style chart areas
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisX.Minimum = 0;
            chart.ChartAreas[0].AxisX.Title = "Observation";
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Title = "Response";

            var series = new Series
            {
                Name = "Baseline Observation",
                Color = System.Drawing.Color.Blue,
                IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Point
            };

            chart.Series.Add(series);

            for (int i = 0; i < blRange.Count; i++)
            {
                chart.Series[0].Points.AddXY(i + 1, blRange[i]);
            }

            chart.Legends.Add(series.Name);

            var series2 = new Series
            {
                Name = "Treatment Observation",
                Color = System.Drawing.Color.Red,
                IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Point
            };

            chart.Series.Add(series2);

            for (int i = 0; i < txRange.Count; i++)
            {
                chart.Series[1].Points.AddXY(i + 1 + blRange.Count, txRange[i]);
            }

            chart.Legends.Add(series2.Name);

            /**  TS Projections **/

            var series3 = new Series
            {
                Name = "Baseline Sen Slope",
                Color = System.Drawing.Color.DarkBlue,
                IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line
            };

            chart.Series.Add(series3);

            for (int i = 0; i < blRange.Count; i++)
            {
                double projection = newBl.Sen * (i + 1) + blInterceptObj.ConoverIntercept;
                chart.Series[2].Points.AddXY(i + 1, projection);
            }

            chart.Legends.Add(series3.Name);

            var series4 = new Series
            {
                Name = "Treatment Sen Slope",
                Color = System.Drawing.Color.DarkRed,
                IsVisibleInLegend = true,
                IsXValueIndexed = false,
                ChartType = SeriesChartType.Line
            };

            chart.Series.Add(series4);

            for (int i = 0; i < txRange.Count; i++)
            {
                double projection = newTx.Sen * (i + 1 + blRange.Count) + txInterceptObj.ConoverIntercept;
                chart.Series[3].Points.AddXY(i + 1 + blRange.Count, projection);
            }

            chart.Legends.Add(series4.Name);

            chart.Legends[0].IsDockedInsideChartArea = true;
            chartWin.Title = "Theil Sen Slope Estimator";
            chartWin.Width = 400;
            chartWin.Height = 300;
            chartWin.Owner = windowRef;

            if (ChartFlag)
            {
                chartWin.Show();
            }
        }
    }
}
