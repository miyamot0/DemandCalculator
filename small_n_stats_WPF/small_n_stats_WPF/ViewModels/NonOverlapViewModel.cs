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
            DefaultFieldsToWhite();

            BaselineBackGround = Brushes.Red;

            mWindow.spreadSheetView.PickRange((inst, range) =>
            {
                if (range.Rows > 1 && range.Cols > 1)
                {
                    BaselineBackGround = Brushes.White;
                    MessageBox.Show("Please select single row or single column selections");
                    return true;
                }

                BaselineRangeString = range.ToString();
                BaselineBackGround = Brushes.White;

                return true;

            }, Cursors.Cross);
        }

        private void LoadInterventionData()
        {
            DefaultFieldsToWhite();

            InterventionBackGround = Brushes.Red;

            mWindow.spreadSheetView.PickRange((inst, range) =>
            {
                if (range.Rows > 1 && range.Cols > 1)
                {
                    InterventionBackGround = Brushes.White;
                    MessageBox.Show("Please select single row or single column selections");
                    return true;
                }

                InterventionRangeString = range.ToString();
                InterventionBackGround = Brushes.White;
                return true;

            }, Cursors.Cross);
        }

        /* For clarify sake, so no multiple red fields */

        private void DefaultFieldsToWhite()
        {
            BaselineBackGround = Brushes.White;
            InterventionBackGround = Brushes.White;
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
