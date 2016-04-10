/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * Small n Stats Application
 * Modeled from conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Models;
using small_n_stats_WPF.Views;
using small_n_stats_WPF.Interfaces;
using small_n_stats_WPF.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using unvell.ReoGrid;

namespace small_n_stats_WPF.ViewModels
{
    class NonOverlapViewModel : ViewModelBase
    {
        public MainWindow mWindow { get; set; }
        public NonOverlapWindow windowRef { get; set; }

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

        private bool pnd = false;
        public bool PND
        {
            get { return pnd; }
            set
            {
                pnd = value;
                OnPropertyChanged("PND");
            }
        }

        private bool pem = false;
        public bool PEM
        {
            get { return pem; }
            set
            {
                pem = value;
                OnPropertyChanged("PEM");
            }
        }

        private bool pand = false;
        public bool PAND
        {
            get { return pand; }
            set
            {
                pand = value;
                OnPropertyChanged("PAND");
            }
        }

        private bool ird = false;
        public bool IRD
        {
            get { return ird; }
            set
            {
                ird = value;
                OnPropertyChanged("IRD");
            }
        }

        private bool phi = false;
        public bool PHI
        {
            get { return phi; }
            set
            {
                phi = value;
                OnPropertyChanged("PHI");
            }
        }

        /* Command Logic */

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand LoadBaselineDataCommand { get; set; }
        public RelayCommand LoadInterventionDataCommand { get; set; }
        public RelayCommand ScoreCommand { get; set; }

        /* Math */

        private SingleCaseMetrics mSingleCase;

        public NonOverlapViewModel()
        {
            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            LoadBaselineDataCommand = new RelayCommand(param => LoadBaselineData(), param => true);
            LoadInterventionDataCommand = new RelayCommand(param => LoadInterventionData(), param => true);

            ScoreCommand = new RelayCommand(param => Score(), param => true);
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        private void ViewLoaded()
        {
            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Loading non-overlap modules...");

            mSingleCase = new SingleCaseMetrics();

            mInterface.SendMessageToOutput("Non-overlap modules loaded.");
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

        private async void Score()
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

            if (PND)
            {
                mInterface.SendMessageToOutput("---------------------------------------------------");
                mInterface.SendMessageToOutput("Percent of Non-Overlapping Data (PND)...");

                double PND = await Task.Factory.StartNew(
                    () => mSingleCase.getPND()
                );

                mInterface.SendMessageToOutput("Results of PND were " + PND.ToString("0.0000"));
            }
            if (PEM)
            {
                mInterface.SendMessageToOutput("---------------------------------------------------");
                mInterface.SendMessageToOutput("Calculating Percent Exceeding Median...");

                PercentExceedingMedianModel PEM = await Task.Factory.StartNew(
                    () => mSingleCase.getPEM()
                );

                mInterface.SendMessageToOutput(string.Format("Results of PEM @ 80% confidence were {0} ( {1} - {2} )",
                    PEM.PEM80.PercentExceedingMedian.ToString("0.000"),
                    PEM.PEM80.LowerBound.ToString("0.000"),
                    PEM.PEM80.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of PEM @ 85% confidence were {0} ( {1} - {2} )",
                    PEM.PEM85.PercentExceedingMedian.ToString("0.000"),
                    PEM.PEM85.LowerBound.ToString("0.000"),
                    PEM.PEM85.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of PEM @ 90% confidence were {0} ( {1} - {2} )",
                    PEM.PEM90.PercentExceedingMedian.ToString("0.000"),
                    PEM.PEM90.LowerBound.ToString("0.000"),
                    PEM.PEM90.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of PEM @ 95% confidence were {0} ( {1} - {2} )",
                    PEM.PEM95.PercentExceedingMedian.ToString("0.000"),
                    PEM.PEM95.LowerBound.ToString("0.000"),
                    PEM.PEM95.UpperBound.ToString("0.000")));
            }
            if (PAND)
            {
                mInterface.SendMessageToOutput("---------------------------------------------------");
                mInterface.SendMessageToOutput("Percent of All Non-Overlapping Data (PAND)...");

                double PAND = await Task.Factory.StartNew(
                    () => mSingleCase.getPAND()
                );

                mInterface.SendMessageToOutput("Results of PAND were " + PAND.ToString("0.000"));
            }
            if (IRD)
            {
                mInterface.SendMessageToOutput("---------------------------------------------------");
                mInterface.SendMessageToOutput("Calculating Improvement Rate Difference...");

                ImprovementRateDifferenceModel IRD = await Task.Factory.StartNew(
                    () => mSingleCase.getIRD()
                );

                mInterface.SendMessageToOutput(string.Format("Results of IRD @ 80% confidence were {0} ( {1} - {2} )",
                    IRD.IRD80.ImprovementRateDifference.ToString("0.000"),
                    IRD.IRD80.LowerBound.ToString("0.000"),
                    IRD.IRD80.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of IRD @ 85% confidence were {0} ( {1} - {2} )",
                    IRD.IRD85.ImprovementRateDifference.ToString("0.000"),
                    IRD.IRD85.LowerBound.ToString("0.000"),
                    IRD.IRD85.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of IRD @ 90% confidence were {0} ( {1} - {2} )",
                    IRD.IRD90.ImprovementRateDifference.ToString("0.000"),
                    IRD.IRD90.LowerBound.ToString("0.000"),
                    IRD.IRD90.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of IRD @ 95% confidence were {0} ( {1} - {2} )",
                    IRD.IRD95.ImprovementRateDifference.ToString("0.000"),
                    IRD.IRD95.LowerBound.ToString("0.000"),
                    IRD.IRD95.UpperBound.ToString("0.000")));
            }
            if (PHI)
            {
                mInterface.SendMessageToOutput("---------------------------------------------------");
                mInterface.SendMessageToOutput("Calculating Pearson's Phi...");

                PearsonPhiModel PHI = await Task.Factory.StartNew(
                    () => mSingleCase.getPHI()
                );

                mInterface.SendMessageToOutput(string.Format("Results of PHI @ 80% confidence were {0} ( {1} - {2} )",
                    PHI.Phi80.PearsonPhi.ToString("0.000"),
                    PHI.Phi80.LowerBound.ToString("0.000"),
                    PHI.Phi80.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of PHI @ 85% confidence were {0} ( {1} - {2} )",
                    PHI.Phi85.PearsonPhi.ToString("0.000"),
                    PHI.Phi85.LowerBound.ToString("0.000"),
                    PHI.Phi85.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of PHI @ 90% confidence were {0} ( {1} - {2} )",
                    PHI.Phi90.PearsonPhi.ToString("0.000"),
                    PHI.Phi90.LowerBound.ToString("0.000"),
                    PHI.Phi90.UpperBound.ToString("0.000")));

                mInterface.SendMessageToOutput(string.Format("Results of PHI @ 95% confidence were {0} ( {1} - {2} )",
                    PHI.Phi95.PearsonPhi.ToString("0.000"),
                    PHI.Phi95.LowerBound.ToString("0.000"),
                    PHI.Phi95.UpperBound.ToString("0.000")));
            }
        }
    }
}
