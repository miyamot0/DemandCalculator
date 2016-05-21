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

    This file uses R.NET Community to leverage interactions with the R program

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

    ============================================================================

    ClosedXML is distributed under this license:

    Copyright (c) 2010 Manuel De Leon

    Permission is hereby granted, free of charge, to any person obtaining a copy of 
    this software and associated documentation files (the "Software"), to deal in the 
    Software without restriction, including without limitation the rights to use, 
    copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the 
    Software, and to permit persons to whom the Software is furnished to do so, 
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all 
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/

using ClosedXML.Excel;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using RDotNet;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace small_n_stats_WPF.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        public MainWindow MainWindow { get; set; }
        Thread loadThread;
        Window window;

        #region Observable Bindings

        private ObservableCollection<RowViewModel> rowViewModels { get; set; } 
        public ObservableCollection<RowViewModel> RowViewModels
        {
            get { return rowViewModels; }
            set
            {
                rowViewModels = value;
                OnPropertyChanged("RowViewModels");
            }
        }

        public string title = "Small n Stats - New File";
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        public bool shuttingDown = false;
        public bool ShuttingDown
        {
            get { return shuttingDown; }
            set
            {
                shuttingDown = value;
                OnPropertyChanged("ShuttingDown");
            }
        }

        #endregion

        #region Commands

        public RelayCommand FileNewCommand { get; set; }
        public RelayCommand FileOpenCommand { get; set; }
        public RelayCommand FileSaveCommand { get; set; }
        public RelayCommand FileSaveAsCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }
        public RelayCommand FileSaveNoDialogCommand { get; set; }

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }

        /* Menu Items */

        public RelayCommand DemandCurveWindowCommand { get; set; }
        public RelayCommand BatchDemandCurveWindowCommand { get; set; }

        public RelayCommand InformationWindowCommand { get; set; }

        REngine engine;

        public RelayCommand GnomeIconLicenseWindowCommand { get; set; }
        public RelayCommand RDotNetLicenseWindowCommand { get; set; }
        public RelayCommand Ggplot2LicenseWindowCommand { get; set; }
        public RelayCommand NlmrtLicenseWindowCommand { get; set; }
        public RelayCommand NlstoolsLicenseWindowCommand { get; set; }
        public RelayCommand RLicenseWindowCommand { get; set; }
        public RelayCommand BeezdemandLicenseWindowCommand { get; set; }
        public RelayCommand LicenseWindowCommand { get; set; }

        /* End Menu Items */

        /* Misc Commands */

        public RelayCommand SaveLogsWindowCommand { get; set; }
        public RelayCommand ClearLogsWindowCommand { get; set; }
        public RelayCommand DeleteSelectedCommand { get; set; }

        /* For Demo Purposes */

        public RelayCommand TestCommand { get; set; }

        /* ^^^ ^^^ ^^^ */

        #endregion

        bool haveFileLoaded = false;
        string path = "";
        public static int RowSpans = 50;
        public static int ColSpans = 100;

        public MainWindowViewModel()
        {
            #region FileCommands

            FileNewCommand = new RelayCommand(param => CreateNewFile(), param => true);
            FileOpenCommand = new RelayCommand(param => OpenFile(), param => true);
            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileSaveAsCommand = new RelayCommand(param => SaveFileAs(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgram(), param => true);

            FileSaveNoDialogCommand = new RelayCommand(param => SaveFileWithoutDialog(), param => true);

            #endregion

            #region LogCommands

            SaveLogsWindowCommand = new RelayCommand(param => SaveLogs(), param => true);
            ClearLogsWindowCommand = new RelayCommand(param => ClearLogs(), param => true);

            #endregion

            #region GridCommands

            DeleteSelectedCommand = new RelayCommand(param => DeleteSelected(), param => true);

            #endregion

            #region TriggerCommands

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);

            #endregion

            #region UICommands

            DemandCurveWindowCommand = new RelayCommand(param => OpenDemandCurveWindow(), param => true);
            BatchDemandCurveWindowCommand = new RelayCommand(param => OpenBatchDemandCurveWindow(), param => true);
            InformationWindowCommand = new RelayCommand(param => OpenInformationWindow(), param => true);

            #endregion

            #region LicenseCommands

            GnomeIconLicenseWindowCommand = new RelayCommand(param => GnomeIconLicenseInformationWindow(), param => true);
            RDotNetLicenseWindowCommand = new RelayCommand(param => RdotNetLicenseInformationWindow(), param => true);
            Ggplot2LicenseWindowCommand = new RelayCommand(param => Ggplot2LicenseInformationWindow(), param => true);
            NlmrtLicenseWindowCommand = new RelayCommand(param => NlmrtLicenseInformationWindow(), param => true);
            NlstoolsLicenseWindowCommand = new RelayCommand(param => NlsToolsLicenseInformationWindow(), param => true);
            RLicenseWindowCommand = new RelayCommand(param => RLicenseInformationWindow(), param => true);
            BeezdemandLicenseWindowCommand = new RelayCommand(param => BeezdemandLicenseInformationWindow(), param => true);
            LicenseWindowCommand = new RelayCommand(param => LicenseInformationWindow(), param => true);

            #endregion

            /* End Menu Items */

            RowViewModels = new ObservableCollection<RowViewModel>();

            ObservableCollection<RowViewModel> temp = new ObservableCollection<RowViewModel>();

            for (int i = 0; i < RowSpans; i++)
            {
                temp.Add(new RowViewModel());
            }

            /* Minor speedup, avoids many UI update calls */

            RowViewModels = new ObservableCollection<RowViewModel>(temp);
        }

        #region UI

        /// <summary>
        /// Update window title through bound object
        /// </summary>
        /// <param name="title">
        /// File name to be used in title (string)
        /// </param>
        public void UpdateTitle(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Loop through selected/highlighted cells, clear cell contents through bound collections
        /// </summary>
        private void DeleteSelected()
        {
            if (MainWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (System.Windows.Controls.DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    var rvm = obj.Item as RowViewModel;

                    if (rvm != null)
                    {
                        int x = RowViewModels.IndexOf(rvm);
                        RowViewModels[x].values[obj.Column.DisplayIndex] = "";
                        RowViewModels[x].ForcePropertyUpdate(obj.Column.DisplayIndex);
                    }
                }
            }
        }

        #endregion

        #region Triggers

        private void ViewLoaded()
        {
            ShuttingDown = false;

            IntroWindow introWindow = new IntroWindow();
            introWindow.Owner = MainWindow;
            introWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            introWindow.Show();

            bool failed = false;

            SendMessageToOutput("Welcome to Demand Curve Calculator!");
            SendMessageToOutput("");
            SendMessageToOutput("All view elements loaded");
            SendMessageToOutput("");

            StreamReader licenseFile = new StreamReader(@"LICENSE.txt");

            string line;

            while ((line = licenseFile.ReadLine()) != null)
            {
                SendMessageToOutput(line);
            }

            SendMessageToOutput("Loading R interop libraries (R.NET Community)");

            try
            {
                REngine.SetEnvironmentVariables();

                SendMessageToOutput("Attempting to link with R installation.");

                engine = REngine.GetInstance();

                SendMessageToOutput("Attempting to Load core binaries...");

                engine.Initialize();
                engine.AutoPrint = false;

            }
            catch (Exception e)
            {
                SendMessageToOutput("R failed to load.  Error code: " + e.ToString());
                failed = true;
            }

            if (failed)
            {
                if (MessageBox.Show("R was not found on your computer.  Do you want to be directed to the R web site for more information?", "R Not Found", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Process.Start("https://www.r-project.org/");
                }
            }
            else
            {
                if (engine.IsRunning)
                {
                    SendMessageToOutput("");
                    SendMessageToOutput("R is found and running");

                    engine.Evaluate("if (!require(ggplot2)) { install.packages('ggplot2', repos = 'http://cran.us.r-project.org') }");
                    engine.Evaluate("if (!require(reshape2)) { install.packages('reshape2', repos = 'http://cran.us.r-project.org') }");
                    engine.Evaluate("if (!require(nlstools)) { install.packages('nlstools', repos = 'http://cran.us.r-project.org') }");
                    engine.Evaluate("if (!require(nlmrt)) { install.packages('nlmrt', repos = 'http://cran.us.r-project.org') }");

                    SendMessageToOutput("All required packages have been found.  Ready to proceed.");
                }
                else
                {
                    SendMessageToOutput("R DLL's not found.");
                }

                SendMessageToOutput("");
                SendMessageToOutput("A listing of all referenced software, with licensing, has been displayed above.");
                SendMessageToOutput("TLDR: Demand Curve Calculator is made possible by the following software.");
                SendMessageToOutput("");
                SendMessageToOutput("R Statistical Package - GPL v2 Licensed. Copyright (C) 2000-16. The R Core Team");
                SendMessageToOutput("nlmrt R Package - GPLv2 Licensed. Copyright (C) 2016. John C. Nash.");
                SendMessageToOutput("nlstools R Package - GPLv2 Licensed. Copyright(C) 2015 Florent Baty and Marie-Laure Delignette - Muller, with contributions from Sandrine Charles, Jean - Pierre Flandrois, and Christian Ritz.");
                SendMessageToOutput("ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham.");
                SendMessageToOutput("reshape2 R Package - MIT Licensed. Copyright (c) 2014, Hadley Wickham.");
                SendMessageToOutput("ClosedXML - MIT Licensed. Copyright (c) 2010 Manuel De Leon.");
                SendMessageToOutput("Gnome Icon Set - GPLv2 Licensed.");
                SendMessageToOutput("RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved.");
                SendMessageToOutput("beezdemand R Package - GPLv2 Licensed. Copyright (c) 2016, Brent Kaplan.");
                SendMessageToOutput("");
                SendMessageToOutput("License information is also provided in Information > Licenses > ... as well as in the install directory of this program (under Resources).");
            }
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
            engine.Dispose();
        }

        #endregion

        #region Licenses

        private void GnomeIconLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Gnome Icons",
                licenseText = Properties.Resources.License_GNOME_icons
            };
            window.Show();
        }

        private void RLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - R Statistical Package",
                licenseText = Properties.Resources.License_R
            };
            window.Show();
        }

        private void NlsToolsLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - nlstools",
                licenseText = Properties.Resources.License_nlstools
            };
            window.Show();
        }

        private void NlmrtLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - nlmrt",
                licenseText = Properties.Resources.License_nlmrt
            };
            window.Show();
        }

        private void Ggplot2LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - ggplot2",
                licenseText = Properties.Resources.License_ggplot2
            };
            window.Show();
        }

        private void RdotNetLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - R.Net",
                licenseText = Properties.Resources.License_RdotNet
            };
            window.Show();
        }

        private void BeezdemandLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Beezdemand",
                licenseText = Properties.Resources.License_Beezdemand
            };
            window.Show();
        }

        private void LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Small n Stats",
                licenseText = Properties.Resources.LICENSE
            };
            window.Show();
        }

        #endregion Licenses

        #region OpenWindows

        private void OpenDemandCurveWindow()
        {
            var mWin = new DemandCurveWindow();
            mWin.Owner = MainWindow;
            mWin.windowTitle.Text = "Demand Curve Analysis";
            mWin.Topmost = true;
            mWin.DataContext = new DemandCurveExponentialViewModel
            {
                mWindow = MainWindow,
                windowRef = mWin
            };
            mWin.Show();
        }

        private void OpenBatchDemandCurveWindow()
        {
            var mWin = new BatchDemandCurveWindow();
            mWin.Owner = MainWindow;
            mWin.windowTitle.Text = "Batch Demand Curve Analysis";
            mWin.Topmost = true;
            mWin.DataContext = new BatchDemandCurveExponentialViewModel
            {
                mWindow = MainWindow,
                windowRef = mWin,
                mViewModel = this
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

        #endregion

        #region FileIO

        private void CreateNewFile()
        {
            loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
            loadThread.SetApartmentState(ApartmentState.STA);
            loadThread.IsBackground = true;
            loadThread.Start();

            RowViewModels.Clear();
            for (int i = 0; i < RowSpans; i++)
            {
                RowViewModels.Add(new RowViewModel());
            }

            UpdateTitle("New File");

            CloseFileUIProgressWindow();
        }

        private void SaveFile()
        {
            if (haveFileLoaded)
            {
                SaveFileWithoutDialog();
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = title;
                saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == true)
                {
                    loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                    loadThread.SetApartmentState(ApartmentState.STA);
                    loadThread.IsBackground = true;
                    loadThread.Start();

                    try
                    {
                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                        UpdateTitle(saveFileDialog1.SafeFileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        haveFileLoaded = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                        Console.WriteLine(e.ToString());
                    }

                    CloseFileUIProgressWindow();
                }
            }
        }

        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.IsBackground = true;
                loadThread.Start();

                try
                {
                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                    UpdateTitle(saveFileDialog1.SafeFileName);

                    path = Path.GetDirectoryName(saveFileDialog1.FileName);

                    haveFileLoaded = true;

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                    haveFileLoaded = false;
                }

                CloseFileUIProgressWindow();

            }
        }

        private void SaveFileWithoutDialog()
        {
            if (haveFileLoaded)
            {
                loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.IsBackground = true;
                loadThread.Start();

                try
                {
                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), Path.Combine(path, title));

                    UpdateTitle(title);

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                }

                CloseFileUIProgressWindow();
            }
        }

        void ShowFileUIProgressWindow()
        {
            window = new ProgressDialog("Processing", "File operations ongoing...");
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
            Dispatcher.Run();
        }

        void CloseFileUIProgressWindow()
        {
            window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(window.Close));
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Spreadsheet Files (XLSX, CSV)|*.xlsx;*.csv";
            openFileDialog1.Title = "Select an Excel File";

            if (openFileDialog1.ShowDialog() == true)
            {
                loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                loadThread.SetApartmentState(ApartmentState.STA);
                loadThread.IsBackground = true;
                loadThread.Start();

                string mExt = Path.GetExtension(openFileDialog1.FileName);

                path = Path.GetDirectoryName(openFileDialog1.FileName);

                try
                {
                    if (mExt.Equals(".xlsx"))
                    {

                        using (var wb = new XLWorkbook(@openFileDialog1.FileName))
                        {
                            
                            var ws = wb.Worksheet(1);
                            var range = ws.RangeUsed();
                            var table = range.AsTable();
                            table.SetShowHeaderRow(false);

                            RowViewModels.Clear();

                            ObservableCollection<RowViewModel> temp = new ObservableCollection<RowViewModel>();

                            foreach (var row in table.Rows())
                            {
                                RowViewModel mModel = new RowViewModel();

                                for (int i = 0; i <= row.CellCount(); i++)
                                {
                                    mModel.values[i] = row.Cell(i).Value.ToString();
                                }

                                //RowViewModels.Add(mModel);
                                temp.Add(mModel);
                            }

                            RowViewModels = new ObservableCollection<RowViewModel>(temp);

                            UpdateTitle(openFileDialog1.SafeFileName);
                            haveFileLoaded = true;
                        }

                    }
                    else if (mExt.Equals(".csv"))
                    {
                        using (TextFieldParser parser = new TextFieldParser(@openFileDialog1.FileName))
                        {
                            parser.TextFieldType = FieldType.Delimited;
                            parser.SetDelimiters(",");

                            RowViewModels.Clear();

                            while (!parser.EndOfData)
                            {
                                string[] fields = parser.ReadFields();

                                RowViewModel mModel = new RowViewModel();
                                for (int i = 0; i < fields.Length && i < 100; i++)
                                {
                                    mModel.values[i] = fields[i];
                                }
                                RowViewModels.Add(mModel);

                            }

                            UpdateTitle(openFileDialog1.SafeFileName);
                            haveFileLoaded = true;
                        }

                    }

                }
                catch 
                {
                    MessageBox.Show("We weren't able to open the file.  Is the target file open or in use?");
                }

                CloseFileUIProgressWindow();
            }
        }

        private void CloseProgram()
        {
            ShuttingDown = true;
        }

        #endregion FileIO

        #region Logging

        public void SendMessageToOutput(string message)
        {
            MainWindow.OutputEvents(message);
        }

        private void SaveLogs()
        {
            MainWindow.SaveLogs();
        }

        private void ClearLogs()
        {
            MainWindow.ClearLogs();
        }

        #endregion Logging

    }
}
