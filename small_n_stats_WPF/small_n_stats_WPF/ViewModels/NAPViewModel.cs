using small_n_stats_WPF.Graphics;
using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Models;
using small_n_stats_WPF.Views;
using small_n_stats_WPF.Interfaces;
using small_n_stats_WPF.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using unvell.ReoGrid;

namespace small_n_stats_WPF.ViewModels
{
    class NAPViewModel : ViewModelBase
    {
        public MainWindow mWindow { get; set; }
        public NAPWindow windowRef { get; set; }

        internal OutputWindowInterface mInterface;

        /* Brushes */

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

        private bool chartROC = false;
        public bool ChartROC
        {
            get { return chartROC; }
            set
            {
                chartROC = value;
                OnPropertyChanged("ChartROC");
            }
        }

        /* Command Logic */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand LoadBaselineDataCommand { get; set; }
        public RelayCommand LoadInterventionDataCommand { get; set; }
        public RelayCommand ScoreNapCommand { get; set; }

        /* Math */

        SingleCaseMetrics mSingleCase;

        public NAPViewModel()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            LoadBaselineDataCommand = new RelayCommand(param => LoadBaselineData(), param => true);
            LoadInterventionDataCommand = new RelayCommand(param => LoadInterventionData(), param => true);

            ScoreNapCommand = new RelayCommand(param => ScoreNap(), param => true);
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        private void ViewLoaded()
        {
            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Loading NAP/ROC modules...");
            
            mSingleCase = new SingleCaseMetrics();

            mInterface.SendMessageToOutput("NAP/ROC modules loaded.");
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

        private async void ScoreNap()
        {
            if (BaselineRangeString.Length < 1 || InterventionRangeString.Length < 1)
            {
                MessageBox.Show("Please select ranges for both phases.");
                return;
            }

            List<double> xRange = mWindow.ParseRange(BaselineRangeString);
            List<double> yRange = mWindow.ParseRange(InterventionRangeString);

            mSingleCase.baselineArray = xRange;
            mSingleCase.treatmentArray = yRange;

            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Calculating Non-Overlap of All Pairs (NAP)...");

            NonoverlapAllPairsModel NAP = await Task.Factory.StartNew(
                () => mSingleCase.getNAP()
            );

            mInterface.SendMessageToOutput(string.Format("Results of NAP @ 80% confidence were {0} ( {1} - {2} )",
                NAP.NAP80.NonoverlapAllPairs.ToString("0.000"),
                NAP.NAP80.LowerBound.ToString("0.000"),
                NAP.NAP80.UpperBound.ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("Results of NAP @ 85% confidence were {0} ( {1} - {2} )",
                NAP.NAP85.NonoverlapAllPairs.ToString("0.000"),
                NAP.NAP85.LowerBound.ToString("0.000"),
                NAP.NAP85.UpperBound.ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("Results of NAP @ 90% confidence were {0} ( {1} - {2} )",
                NAP.NAP90.NonoverlapAllPairs.ToString("0.000"),
                NAP.NAP90.LowerBound.ToString("0.000"),
                NAP.NAP90.UpperBound.ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("Results of NAP @ 95% confidence were {0} ( {1} - {2} )",
                NAP.NAP95.NonoverlapAllPairs.ToString("0.000"),
                NAP.NAP95.LowerBound.ToString("0.000"),
                NAP.NAP95.UpperBound.ToString("0.000")));

            
            if (ChartROC)
            {
                var chartWin = new ChartingWindow();
                Chart chart = chartWin.FindName("MyWinformChart") as Chart;
                chart.Series.Clear();

                List<double> blCopy = new List<double>(xRange);
                List<double> txCopy = new List<double>(yRange);

                // Style chart areas
                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chart.ChartAreas[0].AxisX.Interval = 1.0;
                chart.ChartAreas[0].AxisX.Minimum = 0;
                chart.ChartAreas[0].AxisX.Maximum = txCopy.Count;
                chart.ChartAreas[0].AxisX.Title = "N-th sorted Treatment Observations";
                chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                chart.ChartAreas[0].AxisY.Interval = 1.0;
                chart.ChartAreas[0].AxisY.Minimum = 0;
                chart.ChartAreas[0].AxisY.Maximum = blCopy.Count;
                chart.ChartAreas[0].AxisY.Title = "N-th sorted Baseline Observations";

                var hack = new Series
                {
                    Name = "Baseline Series Dominance",
                    Color = System.Drawing.Color.LightSkyBlue,
                    IsVisibleInLegend = true,
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Area
                };

                chart.Series.Add(hack);

                hack.Points.AddXY(0, blCopy.Count);
                hack.Points.AddXY(txCopy.Count, blCopy.Count);

                var series = new Series
                {
                    Name = "Treatment Series Dominance",
                    Color = System.Drawing.Color.DarkBlue,
                    IsVisibleInLegend = true,
                    IsXValueIndexed = false,
                    ChartType = SeriesChartType.Area
                };

                chart.Series.Add(series);


                blCopy.Sort();
                txCopy.Sort();

                int blN = blCopy.Count;
                int txN = txCopy.Count;

                int n = blN + txN;

                int pairs = blN * txN;
                int over = 0;
                int under = 0;
                int same = 0;

                double comparer;

                int xPos = 0;
                int yPos = 0;
                bool tied = false;

                chart.Series[1].Points.AddXY(xPos, yPos);

                for (int i = 0; i < blN; i++)
                {
                    tied = false;

                    for (int j = 0; j < txN; j++)
                    {
                        comparer = txCopy[j] - blCopy[i];

                        if (comparer > 0)
                        {
                            over++;
                            yPos++;
                        }
                        else if (comparer == 0 || (txCopy[j].ToString() == blCopy[i].ToString()))
                        {
                            same++;
                            tied = true;
                        }
                        else
                        {
                            under++;
                        }
                    }

                    if (tied)
                    {
                        chart.Series[1].Points.AddXY(txCopy.Count - yPos, xPos + 1);
                    }
                    else
                    {
                        chart.Series[1].Points.AddXY(txCopy.Count - yPos, xPos);
                        chart.Series[1].Points.AddXY(txCopy.Count - yPos, xPos + 1);
                    }

                    yPos = 0;
                    xPos++;
                }

                chart.Series[1].Points.AddXY(txCopy.Count, blCopy.Count);

                var series2 = new Series
                {
                    Name = "Dominance Index",
                    Color = System.Drawing.Color.Red,
                    IsVisibleInLegend = true,
                    IsXValueIndexed = false,
                    BorderDashStyle = ChartDashStyle.Dash,
                    ChartType = SeriesChartType.Line
                };

                chart.Series.Add(series2);

                series2.Points.AddXY(0, 0);
                series2.Points.AddXY(txCopy.Count, blCopy.Count);

                chart.Legends.Add(series.Name);
                chart.Legends[0].IsDockedInsideChartArea = true;
                chartWin.Title = "Ordinal Dominance Curve";
                chartWin.Width = 400;
                chartWin.Height = 300;
                chartWin.Owner = windowRef;
                chartWin.Show();
            }
        }
    }
}
