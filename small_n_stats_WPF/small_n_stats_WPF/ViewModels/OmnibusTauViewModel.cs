using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Models;
using small_n_stats_WPF.Views;
using small_n_stats_WPF.Interfaces;
using small_n_stats_WPF.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace small_n_stats_WPF.ViewModels
{
    class OmnibusTauViewModel : ViewModelBase
    {
        public MainWindow mWindow { get; set; }
        public OmnibusTauWindow windowRef { get; set; }

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

        private ObservableCollection<TauUModel> tauUHolder;
        public ObservableCollection<TauUModel> TauUHolder
        {
            get { return tauUHolder; }
            set
            {
                tauUHolder = value;
                OnPropertyChanged("TauUHolder");
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

        private bool correctBaseline = false;
        public bool CorrectBaseline
        {
            get { return correctBaseline; }
            set
            {
                correctBaseline = value;
                OnPropertyChanged("CorrectBaseline");
            }
        }

        /* Command Logic */

        //public RelayCommand RemoveItemCommand { get; set; }

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }
        public RelayCommand LoadBaselineDataCommand { get; set; }
        public RelayCommand LoadInterventionDataCommand { get; set; }
        public RelayCommand CheckBaselineCommand { get; set; }
        public RelayCommand CompareBaselineCommand { get; set; }
        public RelayCommand CalculateOmnibusCommand { get; set; }

        private ICommand _removeItemCommand;
        public ICommand RemoveItemCommand
        {
            get { return _removeItemCommand ?? (_removeItemCommand = new RelayCommand(p => RemoveItem((TauUModel)p))); }
        }

        /* Math */

        TauU mTauMethods;

        public OmnibusTauViewModel()
        {
            //RemoveItemCommand = new RelayCommand(param => RemoveItem(this.RemoveItemCommand), param => true);

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);
            LoadBaselineDataCommand = new RelayCommand(param => LoadBaselineData(), param => true);
            LoadInterventionDataCommand = new RelayCommand(param => LoadInterventionData(), param => true);

            CheckBaselineCommand = new RelayCommand(param => CheckBaseline(), param => true);
            CompareBaselineCommand = new RelayCommand(param => CompareBaseline(), param => true);
            CalculateOmnibusCommand = new RelayCommand(param => CalculateOmnibus(), param => true);
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        private void ViewLoaded()
        {
            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Loading Tau-U modules...");

            mTauMethods = new TauU();

            TauUHolder = new ObservableCollection<TauUModel>();

            mInterface.SendMessageToOutput("Tau-U modules loaded.");
        }

        private void RemoveItem(TauUModel item)
        {
            if (item != null)
                TauUHolder.Remove(item);

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

        private void CheckBaseline()
        {
            List<double> phase1 = mWindow.ParseRange(baselineRangeString);
            TauUModel mTau = mTauMethods.BaselineTrend(phase1, phase1, false);
            mTau.Name = "Baseline: " + baselineRangeString;
            mTau.Range = baselineRangeString;
            mTau.IsChecked = false;

            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Scoring baseline range { " + baselineRangeString + " }");
            mInterface.SendMessageToOutput("Baseline Tau score was " + mTau.TAU.ToString("0.000"));
        }

        private void CompareBaseline()
        {
            List<double> phase1 = mWindow.ParseRange(BaselineRangeString);
            List<double> phase2 = mWindow.ParseRange(InterventionRangeString);

            mInterface.SendMessageToOutput("---------------------------------------------------");

            if (phase1.Count < 3 || phase2.Count < 3)
            {
                mInterface.SendMessageToOutput("Arrays appear short ( less than 3 items in 1 or more arrays ).");
                MessageBox.Show("Too few items were found.  Please review the selected arrays.");
                return;
            }

            TauUModel mTau = mTauMethods.BaselineTrend(phase1, phase2, CorrectBaseline);

            mTau.Name = "Comparisons of {" + BaselineRangeString + "} and {" + InterventionRangeString + "} Corrected: " + CorrectBaseline;
            mTau.IsCorrected = CorrectBaseline;
            mTau.Range = BaselineRangeString;
            mTau.IsChecked = false;

            mInterface.SendMessageToOutput("Comparisons of {" + BaselineRangeString + "} and {" + InterventionRangeString + "} Corrected: " + CorrectBaseline);

            mInterface.SendMessageToOutput(string.Format("S: {0}, Pairs: {1}, Tau: {2}, TauB: {3}, P: {4}",
                mTau.S.ToString("0.000"),
                mTau.Pairs.ToString("0.000"),
                mTau.TAU.ToString("0.000"),
                mTau.TAUB.ToString("0.000"),
                mTau.PValue.ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("@ 85% CI ({0} - {1})",
                mTau.CI_85[0].ToString("0.000"),
                mTau.CI_85[1].ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("@ 90% CI ({0} - {1})",
                mTau.CI_90[0].ToString("0.000"),
                mTau.CI_90[1].ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("@ 95% CI ({0} - {1})",
                mTau.CI_95[0].ToString("0.000"),
                mTau.CI_95[1].ToString("0.000")));

            TauUHolder.Add(mTau);

            CorrectBaseline = false;

            BaselineRangeString = "";
            InterventionRangeString = "";
        }

        private void CalculateOmnibus()
        {
            ObservableCollection<TauUModel> tempHolder = new ObservableCollection<TauUModel>(TauUHolder);

            if (tempHolder.Count < 2)
            {
                mInterface.SendMessageToOutput("At least two Tau-U objects must be selected to construct an omnibus");
                MessageBox.Show("Please select at least two Tau-U objects in the list view (i.e., CTRL+click)");
                return;
            }

            /* Based on Hedges' Optimal Weighting formula */

            double globalTau = 0.0, globalSETau = 0.0;
            double inverseSd = 0.0, inverseSdSum = 0.0;

            foreach(TauUModel model in tempHolder)
            {
                inverseSd = 1.0 / model.SDtau;
                inverseSdSum += inverseSd;
                globalTau += model.TAU * inverseSd;
                globalSETau += model.SDtau * model.SDtau;
            }

            globalTau = globalTau / inverseSdSum;
            globalSETau = System.Math.Sqrt(globalSETau) / tempHolder.Count;

            var z = globalTau / globalSETau;

            var pval = mTauMethods.GetPValueFromUDistribution(z, true);

            double totalInverseVariance = 0.0, totalTauES = 0.0;
            double pairs = 0, ties = 0, S = 0;

            foreach(TauUModel model in tempHolder)
            {
                totalInverseVariance += (1.0 / (model.SDtau * model.SDtau));
                totalTauES += (model.TAU * (1.0 / (model.SDtau * model.SDtau)));

                pairs += model.Pairs;
                ties += model.Ties;
                S += model.S;
            }

            TauUModel omniTau = new TauUModel
            {
                S = S,
                Pairs = pairs,
                TAU = totalTauES / totalInverseVariance,
                TAUB = (S / (pairs * 1.0 - ties * 0.5)),
                VARs = (1.0 / totalInverseVariance),
                SDtau = System.Math.Sqrt(1.0 / totalInverseVariance),
                Z = z,
                PValue = pval,
                CI_85 = new double[] { ((totalTauES / totalInverseVariance) - 1.44 * globalSETau), ((totalTauES / totalInverseVariance) + 1.44 * globalSETau) },
                CI_90 = new double[] { ((totalTauES / totalInverseVariance) - 1.645 * globalSETau), ((totalTauES / totalInverseVariance) + 1.645 * globalSETau) },
                CI_95 = new double[] { ((totalTauES / totalInverseVariance) - 1.96 * globalSETau), ((totalTauES / totalInverseVariance) + 1.96 * globalSETau) },
                IsChecked = false,
                IsCorrected = false
            };

            mInterface.SendMessageToOutput("---------------------------------------------------");
            mInterface.SendMessageToOutput("Omnibus comparison created: ");

            mInterface.SendMessageToOutput(string.Format("Omnibus ES --- S: {0}, Pairs: {1}, Tau: {2}, TauB: {3}, P: {4}",
                omniTau.S.ToString("0.000"),
                omniTau.Pairs.ToString("0.000"),
                omniTau.TAU.ToString("0.000"),
                omniTau.TAUB.ToString("0.000"),
                omniTau.PValue.ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("@ 85% CI ({0} - {1})",
                omniTau.CI_85[0].ToString("0.000"),
                omniTau.CI_85[1].ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("@ 90% CI ({0} - {1})",
                omniTau.CI_90[0].ToString("0.000"),
                omniTau.CI_90[1].ToString("0.000")));

            mInterface.SendMessageToOutput(string.Format("@ 95% CI ({0} - {1})",
                omniTau.CI_95[0].ToString("0.000"),
                omniTau.CI_95[1].ToString("0.000")));

            CorrectBaseline = false;

            BaselineRangeString = "";
            InterventionRangeString = "";
        }

        private void DefaultFieldsToWhite()
        {
            if (BaselineRangeString.Length < 1 || BaselineRangeString.ToLower().Contains("spreadsheet"))
            {
                BaselineBackGround = Brushes.LightGray;
                BaselineRangeString = string.Empty;
            }

            if (InterventionRangeString.Length < 1 || InterventionRangeString.ToLower().Contains("spreadsheet"))
            {
                InterventionBackGround = Brushes.LightGray;
                InterventionRangeString = string.Empty;
            }
        }

    }
}
