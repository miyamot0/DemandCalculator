﻿/* 
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
    EPPlus is distributed under this license:

    Copyright (c) 2016 Jan Källman

    EPPlus is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    EPPlus is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with EPPlus.  If not, see <http://epplus.codeplex.com/license>.

*/

using Microsoft.Win32;
using RDotNet;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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

        private ObservableCollection<MenuItem> recentStuff { get; set; }
        public ObservableCollection<MenuItem> RecentStuff
        {
            get { return recentStuff; }
            set
            {
                recentStuff = value;
                OnPropertyChanged("RecentStuff");
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

        #endregion

        #region Commands

        public RelayCommand FileNewCommand { get; set; }
        public RelayCommand FileOpenCommand { get; set; }
        public RelayCommand FileOpenNoDialogCommand { get; set; }
        public RelayCommand FileSaveCommand { get; set; }
        public RelayCommand FileSaveAsCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }
        public RelayCommand FileSaveNoDialogCommand { get; set; }
        public RelayCommand RecentsClearCommand { get; set; }

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }

        /* Menu Items */

        public RelayCommand DemandCurveWindowCommand { get; set; }
        public RelayCommand BatchDemandCurveWindowCommand { get; set; }
        public RelayCommand InformationWindowCommand { get; set; }

        REngine engine;

        public RelayCommand RDotNetLicenseWindowCommand { get; set; }
        public RelayCommand Ggplot2LicenseWindowCommand { get; set; }
        public RelayCommand NlmrtLicenseWindowCommand { get; set; }
        public RelayCommand NlstoolsLicenseWindowCommand { get; set; }
        public RelayCommand RLicenseWindowCommand { get; set; }
        public RelayCommand BaseEncodeLicenseWindowCommand { get; set; }
        public RelayCommand EPPLicenseWindowCommand { get; set; }
        public RelayCommand BeezdemandLicenseWindowCommand { get; set; }
        public RelayCommand LicenseWindowCommand { get; set; }

        /* End Menu Items */

        /* Misc Commands */

        public RelayCommand SaveLogsWindowCommand { get; set; }
        public RelayCommand ClearLogsWindowCommand { get; set; }
        public RelayCommand DeleteSelectedCommand { get; set; }
        public RelayCommand CutSelectedCommand { get; set; }

        #endregion

        bool haveFileLoaded = false;
        string path = "";
        public static int RowSpans = 50;
        public static int ColSpans = 100;
        private string workingSheet = "";
        string[] recentsArray;

        public MainWindowViewModel()
        {
            #region FileCommands

            FileNewCommand = new RelayCommand(param => CreateNewFile(), param => true);
            FileOpenCommand = new RelayCommand(param => OpenFile(), param => true);
            FileSaveCommand = new RelayCommand(param => SaveFile(), param => true);
            FileSaveAsCommand = new RelayCommand(param => SaveFileAs(), param => true);
            FileCloseCommand = new RelayCommand(param => CloseProgramWindow(param), param => true);

            FileSaveNoDialogCommand = new RelayCommand(param => SaveFileWithoutDialog(), param => true);
            FileOpenNoDialogCommand = new RelayCommand(param => FileOpenNoDialog(param), param => true);


            RecentsClearCommand = new RelayCommand(param => ClearRecents(), param => true);
            
            RecentStuff = new ObservableCollection<MenuItem>();

            recentsArray = Properties.Settings.Default.RecentFiles.Trim().Split(';');

            List<string> workingRecents = recentsArray.Select(item => item).Where(item => item.Trim().Length > 1).ToList();

            if (workingRecents != null && workingRecents.Count > 0)
            {
                RecentStuff.Clear();

                foreach(string recentFileLocation in workingRecents)
                {
                    if (recentFileLocation.Trim().Length < 2)
                    {
                        continue;
                    }

                    RecentStuff.Add(new MenuItem
                    {
                        Header = recentFileLocation,
                        Command = FileOpenNoDialogCommand,
                        CommandParameter = recentFileLocation
                    });
                }
            }

            RecentStuff.Add(new MenuItem
            {
                Header = "Clear Recents",
                Command = RecentsClearCommand
            });

            #endregion

            #region LogCommands

            SaveLogsWindowCommand = new RelayCommand(param => SaveLogs(), param => true);
            ClearLogsWindowCommand = new RelayCommand(param => ClearLogs(), param => true);

            #endregion

            #region GridCommands

            DeleteSelectedCommand = new RelayCommand(param => DeleteSelected(), param => true);
            CutSelectedCommand = new RelayCommand(param => CutSelected(), param => true);

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
            
            RDotNetLicenseWindowCommand = new RelayCommand(param => RdotNetLicenseInformationWindow(), param => true);
            Ggplot2LicenseWindowCommand = new RelayCommand(param => Ggplot2LicenseInformationWindow(), param => true);
            NlmrtLicenseWindowCommand = new RelayCommand(param => NlmrtLicenseInformationWindow(), param => true);
            NlstoolsLicenseWindowCommand = new RelayCommand(param => NlsToolsLicenseInformationWindow(), param => true);
            RLicenseWindowCommand = new RelayCommand(param => RLicenseInformationWindow(), param => true);

            BaseEncodeLicenseWindowCommand = new RelayCommand(param => BaseEncodeLicenseInformationWindow(), param => true);
            EPPLicenseWindowCommand = new RelayCommand(param => EPPLicenseWindow(), param => true);

            BeezdemandLicenseWindowCommand = new RelayCommand(param => BeezdemandLicenseInformationWindow(), param => true);
            LicenseWindowCommand = new RelayCommand(param => LicenseInformationWindow(), param => true);

        #endregion

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
        /// Clears the recents list, saving a blank string to settings
        /// </summary>
        private void ClearRecents()
        {
            Properties.Settings.Default.RecentFiles = "";
            Properties.Settings.Default.Save();

            RecentStuff.Clear();
            RecentStuff.Add(new MenuItem
            {
                Header = "Clear Recents",
                Command = RecentsClearCommand
            });
        }

        /// <summary>
        /// Adds a recently opened/saved file to the recent lists, if not already present
        /// </summary>
        /// <param name="filePath">
        /// Path to recently opened/saved file
        /// </param>
        private void AddToRecents(string filePath)
        {
            recentsArray = Properties.Settings.Default.RecentFiles.Split(';');

            List<string> workingRecents = recentsArray.Select(item => item).Where(item => item.Trim().Length > 1).ToList();

            if (!workingRecents.Contains(filePath))
            {
                workingRecents.Add(filePath);
                Properties.Settings.Default.RecentFiles = string.Join(";", workingRecents.ToArray());
                Properties.Settings.Default.Save();

                RecentStuff.Clear();

                foreach (string recentFileLocation in workingRecents)
                {
                    if (recentFileLocation.Trim().Length < 2)
                    {
                        continue;
                    }

                    RecentStuff.Add(new MenuItem
                    {
                        Header = recentFileLocation,
                        Command = FileOpenNoDialogCommand,
                        CommandParameter = recentFileLocation
                    });
                }

                RecentStuff.Add(new MenuItem
                {
                    Header = "Clear Recents",
                    Command = RecentsClearCommand
                });
            }
        }

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

        /// <summary>
        /// Cut cells after copying to clipboard
        /// </summary>
        private void CutSelected()
        {
            if (MainWindow.dataGrid.SelectedCells.Count > 0)
            {
                List<string> holdPreClip = new List<string>();

                foreach (DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    var rvm = obj.Item as RowViewModel;

                    if (rvm != null)
                    {
                        int x = RowViewModels.IndexOf(rvm);
                        holdPreClip.Add(RowViewModels[x].values[obj.Column.DisplayIndex]);
                    }
                }

                string holdClip = string.Join("\t", holdPreClip);
                Clipboard.SetText(holdClip);

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

        /// <summary>
        /// Loaded event trigger
        /// </summary>
        private void ViewLoaded()
        {
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
                    engine.Evaluate("if (!require(base64enc)) { install.packages('base64enc', repos = 'http://cran.us.r-project.org') }");

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
                SendMessageToOutput("base64enc R Package - GPLv2+ Licensed. Copyright (c) 2015, Simon Urbanek.");
                SendMessageToOutput("EPPlus - GPLv2 Licensed. Copyright (c) 2016 Jan Källman.");
                SendMessageToOutput("RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved.");
                SendMessageToOutput("beezdemand R Package - GPLv2 Licensed. Copyright (c) 2016, Brent Kaplan.");
                SendMessageToOutput("");
                SendMessageToOutput("License information is also provided in Information > Licenses > ... as well as in the install directory of this program (under Resources).");
            }
        }

        /// <summary>
        /// Closed event trigger
        /// </summary>
        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
            engine.Dispose();
        }

        #endregion

        #region Licenses

        /// <summary>
        /// License window
        /// </summary>
        private void RLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - R Statistical Package",
                licenseText = Properties.Resources.License_R
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void EPPLicenseWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License (GPLv2) - EPPlus",
                licenseText = Properties.Resources.License_EPPlus
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void NlsToolsLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - nlstools",
                licenseText = Properties.Resources.License_nlstools
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void NlmrtLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - nlmrt",
                licenseText = Properties.Resources.License_nlmrt
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void Ggplot2LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - ggplot2",
                licenseText = Properties.Resources.License_ggplot2
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void BaseEncodeLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License (GPLv2+) - base64enc",
                licenseText = Properties.Resources.License_base64enc
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void RdotNetLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - R.Net",
                licenseText = Properties.Resources.License_RdotNet
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void BeezdemandLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Beezdemand",
                licenseText = Properties.Resources.License_Beezdemand
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void LicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Small n Stats",
                licenseText = Properties.Resources.LICENSE
            };
            window.Owner = MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        #endregion Licenses

        #region OpenWindows

        /// <summary>
        /// Single mode analysis window
        /// </summary>
        private void OpenDemandCurveWindow()
        {
            var mWin = new DemandCurveWindow();
            mWin.Owner = MainWindow;
            mWin.windowTitle.Text = "Demand Curve Analysis";
            mWin.DataContext = new DemandCurveExponentialViewModel
            {
                mWindow = MainWindow,
                windowRef = mWin
            };
            mWin.Show();
        }

        /// <summary>
        /// Batch mode analysis window
        /// </summary>
        private void OpenBatchDemandCurveWindow()
        {
            var mWin = new BatchDemandCurveWindow();
            mWin.Owner = MainWindow;
            mWin.windowTitle.Text = "Batch Demand Curve Analysis";
            mWin.DataContext = new BatchDemandCurveExponentialViewModel
            {
                mWindow = MainWindow,
                windowRef = mWin
            };
            mWin.Show();
        }

        /// <summary>
        /// Information window
        /// </summary>
        private void OpenInformationWindow()
        {
            var mWin = new InformationWindow();
            mWin.Owner = MainWindow;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mWin.Show();
        }

        #endregion

        #region FileIO

        /// <summary>
        /// Creates new spreadsheet, not really "file"
        /// </summary>
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
            workingSheet = "Sheet1";

            haveFileLoaded = false;

            CloseFileUIProgressWindow();
        }

        /// <summary>
        /// Saves file, usually from Ctrl+S binding
        /// </summary>
        private void SaveFile()
        {
            MainWindow.dataGrid.CommitEdit();

            if (haveFileLoaded)
            {
                SaveFileWithoutDialog();
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.FileName = title;
                saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == true)
                {
                    try
                    {
                        string mExt = Path.GetExtension(saveFileDialog1.FileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        if (mExt.Equals(".xlsx"))
                        {
                            loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                            loadThread.SetApartmentState(ApartmentState.STA);
                            loadThread.IsBackground = true;
                            loadThread.Start();

                            OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                            CloseFileUIProgressWindow();
                        }
                        else if (mExt.Equals(".csv"))
                        {
                            OpenXMLHelper.ExportToCSV(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);
                        }

                        UpdateTitle(saveFileDialog1.SafeFileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        haveFileLoaded = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                        Console.WriteLine(e.ToString());
                    }

                    workingSheet = "Demand Analysis Calculations";
                }
            }
        }

        /// <summary>
        /// Saves file with a dialog 
        /// </summary>
        private void SaveFileAs()
        {
            MainWindow.dataGrid.CommitEdit();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.FileName = title;
            saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == true)
            {
                string mExt = Path.GetExtension(saveFileDialog1.FileName);

                path = Path.GetDirectoryName(saveFileDialog1.FileName);

                try
                {
                    if (mExt.Equals(".xlsx"))
                    {
                        loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                        loadThread.SetApartmentState(ApartmentState.STA);
                        loadThread.IsBackground = true;
                        loadThread.Start();

                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                        CloseFileUIProgressWindow();
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        OpenXMLHelper.ExportToCSV(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);
                    }

                    workingSheet = "Demand Analysis Calculations";

                    UpdateTitle(saveFileDialog1.SafeFileName);

                    path = Path.GetDirectoryName(saveFileDialog1.FileName);

                    haveFileLoaded = true;


                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                    Console.WriteLine(e.ToString());
                    haveFileLoaded = false;
                }

            }
        }

        /// <summary>
        /// Saves file without a dialog call
        /// </summary>
        private void SaveFileWithoutDialog()
        {
            MainWindow.dataGrid.CommitEdit();

            if (haveFileLoaded)
            {
                try
                {
                    string mExt = Path.GetExtension(Path.Combine(path, title));

                    path = Path.GetDirectoryName(Path.Combine(path, title));

                    if (mExt.Equals(".xlsx"))
                    {
                        loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
                        loadThread.SetApartmentState(ApartmentState.STA);
                        loadThread.IsBackground = true;
                        loadThread.Start();

                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), Path.Combine(path, title));

                        CloseFileUIProgressWindow();
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        OpenXMLHelper.ExportToCSV(new ObservableCollection<RowViewModel>(RowViewModels), Path.Combine(path, title));
                    }

                    UpdateTitle(title);

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                    Console.WriteLine(e.ToString());
                }
            }
        }

        /// <summary>
        /// Opens a file with a dialog
        /// </summary>
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
                        ObservableCollection<RowViewModel> temp = OpenXMLHelper.ReadFromExcelFile(openFileDialog1.FileName, out workingSheet);
                        RowViewModels = new ObservableCollection<RowViewModel>(temp);

                        UpdateTitle(openFileDialog1.SafeFileName);
                        haveFileLoaded = true;
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        ObservableCollection<RowViewModel> temp = OpenXMLHelper.ReadFromCSVFile(openFileDialog1.FileName);
                        RowViewModels = new ObservableCollection<RowViewModel>(temp);

                        UpdateTitle(openFileDialog1.SafeFileName);
                        haveFileLoaded = true;
                    }

                    AddToRecents(@openFileDialog1.FileName);
                }
                catch (IOException e)
                {
                    CloseFileUIProgressWindow();
                    Console.WriteLine(e.ToString());
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                }
                catch (Exception e)
                {
                    CloseFileUIProgressWindow();
                    Console.WriteLine(e.ToString());
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                }

                CloseFileUIProgressWindow();
            }
        }

        /// <summary>
        /// Opens a file without a dialog window
        /// </summary>
        /// <param name="filePath">
        /// path to the file to be opened
        /// </param>
        private void OpenFileNoDialog(string filePath)
        {
            loadThread = new Thread(new ThreadStart(ShowFileUIProgressWindow));
            loadThread.SetApartmentState(ApartmentState.STA);
            loadThread.IsBackground = true;
            loadThread.Start();

            string mExt = Path.GetExtension(@filePath);

            path = Path.GetDirectoryName(@filePath);

            try
            {
                if (mExt.Equals(".xlsx"))
                {
                    ObservableCollection<RowViewModel> temp = OpenXMLHelper.ReadFromExcelFile(filePath, out workingSheet);
                    RowViewModels = new ObservableCollection<RowViewModel>(temp);

                    UpdateTitle(Path.GetFileName(filePath));
                    haveFileLoaded = true;
                }
                else if (mExt.Equals(".csv"))
                {
                    ObservableCollection<RowViewModel> temp = OpenXMLHelper.ReadFromCSVFile(@filePath);
                    RowViewModels = new ObservableCollection<RowViewModel>(temp);

                    UpdateTitle(Path.GetFileName(filePath));
                    haveFileLoaded = true;
                }

                AddToRecents(@filePath);
            }
            catch (IOException e)
            {
                CloseFileUIProgressWindow();
                Console.WriteLine(e.ToString());
                MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
            }
            catch (Exception e)
            {
                CloseFileUIProgressWindow();
                Console.WriteLine(e.ToString());
                MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
            }

            CloseFileUIProgressWindow();
        }

        /// <summary>
        /// Method for opening file w/o dialog
        /// </summary>
        /// <param name="param">
        /// Command parameter 
        /// </param>
        private void FileOpenNoDialog(object param)
        {
            string path = param as string;

            if (path != null)
            {
                OpenFileNoDialog(path);
            }
        }

        /// <summary>
        /// Shows progress bar on another thread
        /// </summary>
        void ShowFileUIProgressWindow()
        {
            window = new ProgressDialog("Processing", "File operations ongoing...");
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.Show();
            Dispatcher.Run();
        }

        /// <summary>
        /// Closes progress bar on another thread
        /// </summary>
        void CloseFileUIProgressWindow()
        {
            window.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(window.Close));
        }

        /// <summary>
        /// Shutdown event
        /// </summary>
        /// <param name="param"></param>
        private void CloseProgramWindow(object param)
        {
            var windowObj = param as Window;

            if (windowObj != null)
            {
                windowObj.Close();
            }
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
