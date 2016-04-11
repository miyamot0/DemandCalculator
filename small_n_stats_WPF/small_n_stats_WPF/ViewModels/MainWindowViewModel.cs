/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * Small n Stats Application
 * Modeled from conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using small_n_stats_WPF.Interfaces;
using small_n_stats_WPF.Mathematics;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace small_n_stats_WPF.ViewModels
{
    class MainWindowViewModel : ViewModelBase, OutputWindowInterface
    {
        public MainWindow MainWindow { get; set; }
        public SpreadsheetInterface _interface { get; set; }

        public RelayCommand FileNewCommand { get; set; }
        public RelayCommand FileOpenCommand { get; set; }
        public RelayCommand FileSaveCommand { get; set; }
        public RelayCommand FileSaveAsCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }

        public RelayCommand FileSaveNoDialogCommand { get; set; }

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }

        /* Menu Items */

        public RelayCommand DiscountingWindowCommand { get; set; }
        public RelayCommand DiscountingComparisonWindowCommand { get; set; }
        public RelayCommand EscalationWindowCommand { get; set; }
        public RelayCommand DemandCurveWindowCommand { get; set; }

        public RelayCommand TheilSenWindowCommand { get; set; }
        public RelayCommand OmnibusTauWindowCommand { get; set; }
        public RelayCommand NAPWindowCommand { get; set; }
        public RelayCommand NonOverlapWindowCommand { get; set; }

        public RelayCommand InformationWindowCommand { get; set; }


        public RelayCommand ReogridLicenseWindowCommand { get; set; }
        public RelayCommand GnomeIconLicenseWindowCommand { get; set; }
        public RelayCommand TillyLicenseWindowCommand { get; set; }

        /* End Menu Items */

        public RichTextBox mTextBox { get; set; }
        private FlowDocument fd;
        public ScrollViewer sv;

        /* Logic */

        bool haveFileLoaded = false;
        string title = "New File";
        string path = "";

        /* For Demo Purposes */

        public RelayCommand TestCommand { get; set; }

        /* ^^^ ^^^ ^^^ */

        public MainWindowViewModel()
        {
            FileNewCommand = new RelayCommand(param => CreateNewFile(), param => true);
            FileOpenCommand = new RelayCommand(param => OpenFile(), param => true);
            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileSaveAsCommand = new RelayCommand(param => SaveFileAs(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgram(), param => true);

            FileSaveNoDialogCommand = new RelayCommand(param => SaveFileWithoutDialog(), param => true);

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);

            /* Menu Items */

            TheilSenWindowCommand = new RelayCommand(param => OpenTheilSenWindow(), param => true);
            OmnibusTauWindowCommand = new RelayCommand(param => OpenOmnibusTauWindow(), param => true);
            NAPWindowCommand = new RelayCommand(param => OpenNAPWindow(), param => true);
            NonOverlapWindowCommand = new RelayCommand(param => OpenNonOverlapWindow(), param => true);

            InformationWindowCommand = new RelayCommand(param => OpenInformationWindow(), param => true);
            ReogridLicenseWindowCommand = new RelayCommand(param => ReogridLicenseInformationWindow(), param => true);
            GnomeIconLicenseWindowCommand = new RelayCommand(param => GnomeIconLicenseInformationWindow(), param => true);
            TillyLicenseWindowCommand = new RelayCommand(param => BenTillyLicenseInformationWindow(), param => true);

            /* End Menu Items */
        }

        private void ViewLoaded()
        {
            mTextBox = MainWindow.outputWindow;
            sv = MainWindow.Scroller;
            SendMessageToOutput("---------------------------------------------------");
            SendMessageToOutput("Welcome to the Small n Stats - Effect Size Calculator");
            SendMessageToOutput("The Effect Size Calculator utilizes several licensed components for its interface and design.");

            SendMessageToOutput("");
            SendMessageToOutput("These works include:");
            SendMessageToOutput("");
            SendMessageToOutput("Reogrid WPF Spreadsheet controls. Copyright (c) 2012-2015 unvell.com, all rights reserved. Licensed under FREE license.");
            SendMessageToOutput("GNOME Icon Theme. Copyright (c) 2012-2015 GNOME Project, all rights reserved. Licensed under CCbySA 3.0.  No modifications performed.");
            SendMessageToOutput("GetPValueFromUDistribution. Copyright (c) 2008 Ben Tilly <btilly@gmail.com>, all rights reserved. Original Perl version by Michael Kospach <mike.perl@gmx.at> Nice formating, simplification and bug repair by Matthias Trautner Kromann <mtk@id.cbs.dk>.  Licensed under GPLv1+");
            SendMessageToOutput("Rich Parker, Kim Vannest and Ozgur Gonen of Single Case Research. Functionality modeled around earlier work using www.SingleCaseResearch.org online calculators.");
            SendMessageToOutput("");
            SendMessageToOutput("Respective licenses available under Information > Licenses.");
            
            List<double> xRange = new List<double> { 3, 5, 6, 4, 3, 5, 6, 7, 5, 6, 9 };
            List<double> yRange = new List<double> { 4,4,6,7,8,6,5,8,9,9,9,9,9,9 };
            SingleCaseMetrics mSingleCase = new SingleCaseMetrics();
            mSingleCase.baselineArray = yRange;
            mSingleCase.treatmentArray = xRange;

            var output = mSingleCase.getIRD();


        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        private void ReogridLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - Reogrid",
                licenseText = Properties.Resources.License_ReogridSpreadsheet
            };
            window.Show();
        }

        private void GnomeIconLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - Gnome Icons (CC by SA 3.0)",
                licenseText = Properties.Resources.COPYING_CCBYSA3
            };
            window.Show();
        }

        private void BenTillyLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new ViewModelLicense
            {
                licenseTitle = "License - Ben Tilly (GPLv2)",
                licenseText = Properties.Resources.License_Ben_Tilly
            };
            window.Show();
        }

        private void OpenNonOverlapWindow()
        {
            var mWin = new NonOverlapWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new NonOverlapViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenInformationWindow()
        {
            var mWin = new InformationWindow();
            mWin.Owner = MainWindow;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mWin.Topmost = true;
            mWin.Show();
        }

        private void OpenNAPWindow()
        {
            var mWin = new NAPWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new NAPViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenOmnibusTauWindow()
        {
            var mWin = new OmnibusTauWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new OmnibusTauViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenTheilSenWindow()
        {
            var mWin = new TheilSenWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new TheilSenViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        /*

        private void OpenDemandCurveWindow()
        {
            var mWin = new DemandCurveWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new DemandCurveViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenDiscountingWindow()
        {
            var mWin = new DiscountingWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new DiscountingViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenDiscountingComparisonWindow()
        {
            var mWin = new ModelSimulationWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new ModelSimulationViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenEscalationWindow()
        {
            var mWin = new EscalationWindow();
            mWin.Owner = MainWindow;
            mWin.Topmost = true;
            mWin.DataContext = new EscalationViewModel
            {
                mWindow = MainWindow,
                mInterface = this,
                windowRef = mWin
            };
            mWin.Show();
        }
        */

        private void CreateNewFile()
        {
            title = "New File";
            haveFileLoaded = _interface.NewFile();
        }

        private void SaveFile()
        {
            if (haveFileLoaded)
            {
                SaveFileWithoutDialog();
            }
            else
            {
                title = _interface.SaveFileWithDialog(title);

                if (title != null)
                {
                    haveFileLoaded = true;
                }

            }
        }

        private void SaveFileAs()
        {
            title = _interface.SaveFileAs(title);

            if (title != null)
            {
                haveFileLoaded = true;
            }
        }

        private void SaveFileWithoutDialog()
        {
            if (haveFileLoaded)
            {
                _interface.SaveFile(path, title);
            }
        }

        private void OpenFile()
        {
            string[] returned = _interface.OpenFile();

            if (returned != null)
            {
                title = returned[0];
                path = returned[1];
            }
        }

        private void CloseProgram()
        {
            _interface.ShutDown();
        }

        public void ParagraphReporting(Paragraph results)
        {

            fd = mTextBox.Document;
            fd.Blocks.Add(results);
            mTextBox.ScrollToEnd();
            sv.ScrollToEnd();
        }

        private void CallBack()
        {
            _interface.UpdateTitle(title);
        }

        public void SendMessageToOutput(string message)
        {
            Paragraph para = new Paragraph();
            para.Inlines.Add(message);
            ParagraphReporting(para);
        }
    }
}
