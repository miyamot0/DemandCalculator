/* 
    Copyright 2016 Shawn Gilroy

    This file is part of Small N Stats.

    Small N Stats is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 2.

    Small N Stats is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Small N Stats.  If not, see <http://www.gnu.org/licenses/gpl-2.0.html>.

*/

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.VisualBasic.FileIO;
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
using System.Windows;

namespace small_n_stats_WPF.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        public MainWindow MainWindow { get; set; }

        #region Observable Bindings

        public ObservableCollection<RowViewModel> RowViewModels { get; set; }

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
        public RelayCommand LicenseWindowCommand { get; set; }

        /* End Menu Items */

        /* Misc Commands */

        public RelayCommand SaveLogsWindowCommand { get; set; }
        public RelayCommand ClearLogsWindowCommand { get; set; }

        public RelayCommand DeleteSelectedCommand { get; set; }

        /* Logic */

        bool haveFileLoaded = false;
        string path = "";
        public static int RowSpans = 50;
        public static int ColSpans = 100;

        /* For Demo Purposes */

        public RelayCommand TestCommand { get; set; }

        /* ^^^ ^^^ ^^^ */

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

        public void UpdateTitle(string title)
        {
            Title = title;
        }

        // TODO MVVM this

        private void DeleteSelected()
        {
            if (MainWindow.dataGrid.SelectedCells.Count > 0)
            {
                foreach (System.Windows.Controls.DataGridCellInfo obj in MainWindow.dataGrid.SelectedCells)
                {
                    int x = (RowViewModels.IndexOf((RowViewModel)obj.Item));
                    RowViewModels[x].values[obj.Column.DisplayIndex] = "";
                    RowViewModels[x].ForcePropertyUpdate(obj.Column.DisplayIndex);
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

            SendMessageToOutput("Welcome to Small n Stats demands analysis!");
            SendMessageToOutput("All view elements loaded");

            SendMessageToOutput("Loading R interop libraries (R.Net.Community)... ");

            try
            {
                REngine.SetEnvironmentVariables();

                SendMessageToOutput("Displaying R.Net.Community License:");
                SendMessageToOutput("");
                SendMessageToOutput("R.Net.Community version 1.6.5, Copyright 2011-2014 RecycleBin, Copyright 2014-2015 CSIRO");
                SendMessageToOutput("R.Net.Community comes with ABSOLUTELY NO WARRANTY; for details select Information > Licenses > R.Net");
                SendMessageToOutput("This is free software, and you are welcome to redistribute it under certain conditions; see license for details.");
                SendMessageToOutput("");
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
                    SendMessageToOutput("Linking to R (R Statistical Package)");
                    SendMessageToOutput("Displaying R License:");

                    /* Interactive post for R */

                    SendMessageToOutput("");
                    SendMessageToOutput("");
                    SendMessageToOutput("R Copyright (C) 2016 R Core Team");
                    SendMessageToOutput("This program comes with ABSOLUTELY NO WARRANTY;");
                    SendMessageToOutput("This is free software, and you are welcome to redistribute it");
                    SendMessageToOutput("under certain conditions; for details select Information > Licenses > R.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    SendMessageToOutput("Checking for required packages: ");
                    /* Loading R packages for analyses */

                    engine.Evaluate("if (!require(ggplot2)) { install.packages('ggplot2', repos = 'http://cran.us.r-project.org') }");
                    /* Interactive post for ggplot2 package */

                    SendMessageToOutput("Package ggplot2 found/loaded");
                    SendMessageToOutput("Displaying ggplot2 License:");
                    SendMessageToOutput("ggplot2 Copyright (C) 2016 Hadley Wickham, Winston Chang");
                    SendMessageToOutput("This program comes with ABSOLUTELY NO WARRANTY;");
                    SendMessageToOutput("This is free software, and you are welcome to redistribute it");
                    SendMessageToOutput("under certain conditions; for details select Information > Licenses > ggplot2.");
                    SendMessageToOutput("H. Wickham. ggplot2: Elegant Graphics for Data Analysis. Springer-Verlag New York, 2009.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    engine.Evaluate("if (!require(nlstools)) { install.packages('nlstools', repos = 'http://cran.us.r-project.org') }");
                    /* Interactive post for nlstools package */

                    SendMessageToOutput("Package nlstools found/loaded");
                    SendMessageToOutput("Displaying nlstools License:");
                    SendMessageToOutput("nlstools Copyright (C) 2015 Florent Baty and Marie-Laure Delignette-Muller, with contributions " +
                        "from Sandrine Charles, Jean-Pierre Flandrois, and Christian Ritz");
                    SendMessageToOutput("This program comes with ABSOLUTELY NO WARRANTY;");
                    SendMessageToOutput("This is free software, and you are welcome to redistribute it");
                    SendMessageToOutput("under certain conditions; for details select Information > Licenses > nlstools.");
                    SendMessageToOutput("Florent Baty, Christian Ritz, Sandrine Charles, Martin Brutsche, Jean-Pierre Flandrois, Marie-Laure Delignette-Muller (2015). A Toolbox for Nonlinear Regression in R: The Package nlstools. Journal of Statistical Software, 66(5), 1-21. URL http://www.jstatsoft.org/v66/i05/.");
                    SendMessageToOutput("");
                    SendMessageToOutput("");

                    engine.Evaluate("if (!require(nlmrt)) { install.packages('nlmrt', repos = 'http://cran.us.r-project.org') }");
                    /* Interactive post for nlmrt package */

                    SendMessageToOutput("Package nlmrt found");
                    SendMessageToOutput("Displaying nlmrt License:");
                    SendMessageToOutput("# https://cran.r-project.org/web/packages/nlmrt/index.html");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# Copyright (C) 2016. John C. Nash.");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# This program is free software; you can redistribute it and/or modify");
                    SendMessageToOutput("# it under the terms of the GNU General Public License as published by");
                    SendMessageToOutput("# the Free Software Foundation; either version 2 of the License, or");
                    SendMessageToOutput("#  (at your option) any later version.");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# This program is distributed in the hope that it will be useful,");
                    SendMessageToOutput("# but WITHOUT ANY WARRANTY; without even the implied warranty of");
                    SendMessageToOutput("# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the");
                    SendMessageToOutput("# GNU General Public License for more details.");
                    SendMessageToOutput("#");
                    SendMessageToOutput("# A copy of the GNU General Public License is available at");
                    SendMessageToOutput("# http://www.r-project.org/Licenses/");
                    SendMessageToOutput("");
                    SendMessageToOutput("");
                    
                    SendMessageToOutput("All required packages have been found.  Ready to proceed.");
                }
                else
                {
                    SendMessageToOutput("R DLL's not found.");
                }

                SendMessageToOutput("A listing of all referenced software, with licensing, has been displayed above.");
                SendMessageToOutput("TLDR: Small n Stats - Demand Analysis is made possible by the following software.");
                SendMessageToOutput("");
                SendMessageToOutput("R Statistical Package - GPLv2 Licensed. Copyright (C) 2000-16. The R Core Team");
                SendMessageToOutput("ggplot2 R Package - GPLv2 Licensed. Copyright (c) 2016, Hadley Wickham.");
                SendMessageToOutput("Gnome Icon Set - GPLv2 Licensed.");
                SendMessageToOutput("RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved.");
                SendMessageToOutput("");
                SendMessageToOutput("");

                SendMessageToOutput("License information is also provided in Information > Licenses > ... as well as");
                SendMessageToOutput("in the install directory of this program (under Resources).");
                SendMessageToOutput("");
                SendMessageToOutput("");
            }
        }

        private void ViewClosed()
        {
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Licenses

        private void GnomeIconLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Gnome Icons (GPLv2)",
                licenseText = Properties.Resources.License_GNOME_icons
            };
            window.Show();
        }

        private void RLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - R Statistical Package (GPLv2)",
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
            mWin.windowTitle.Text = "Demand Curve Analysis (Exponential)";
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
            mWin.windowTitle.Text = "Batch Demand Curve Analysis (Exponential)";
            mWin.Topmost = true;
            mWin.DataContext = new BatchDemandCurveExponentialViewModel
            {
                mWindow = MainWindow,
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

        #endregion

        #region FileIO

        private void CreateNewFile()
        {
            ProgressBarDialog pd = new ProgressBarDialog();
            pd.Owner = MainWindow;
            pd.Title = "Starting New File...";
            pd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            pd.Show();

            RowViewModels.Clear();
            for (int i = 0; i < RowSpans; i++)
            {
                RowViewModels.Add(new RowViewModel());
            }

            UpdateTitle("New File");

            pd.Close();
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

                    try
                    {
                        ProgressBarDialog pd = new ProgressBarDialog();
                        pd.Owner = MainWindow;
                        pd.Title = "Saving File...";
                        pd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        pd.Show();

                        OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                        UpdateTitle(saveFileDialog1.SafeFileName);

                        pd.Close();

                        haveFileLoaded = true;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                        Console.WriteLine(e.ToString());
                    }

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
                try
                {
                    ProgressBarDialog pd = new ProgressBarDialog();
                    pd.Owner = MainWindow;
                    pd.Title = "Saving File...";
                    pd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    pd.Show();

                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), saveFileDialog1.FileName);

                    UpdateTitle(saveFileDialog1.SafeFileName);

                    pd.Close();

                    haveFileLoaded = true;

                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                    haveFileLoaded = false;
                }

            }
        }

        private void SaveFileWithoutDialog()
        {
            if (haveFileLoaded)
            {
                try
                {
                    ProgressBarDialog pd = new ProgressBarDialog();
                    pd.Owner = MainWindow;
                    pd.Title = "Saving File...";
                    pd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    pd.Show();

                    OpenXMLHelper.ExportToExcel(new ObservableCollection<RowViewModel>(RowViewModels), Path.Combine(path, title));

                    UpdateTitle(title);

                    pd.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                SaveFileAs();
            }
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Spreadsheet Files (XLSX, CSV)|*.xlsx;*.csv";
            openFileDialog1.Title = "Select an Excel File";

            if (openFileDialog1.ShowDialog() == true)
            {
                ProgressBarDialog pd = new ProgressBarDialog();
                pd.Owner = MainWindow;
                pd.Title = "Opening File...";
                pd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                pd.Show();

                string mExt = Path.GetExtension(openFileDialog1.FileName);
                path = Path.GetDirectoryName(openFileDialog1.FileName);

                try
                {

                    if (mExt.Equals(".xlsx"))
                    {
                        using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(@openFileDialog1.FileName, false))
                        {
                            WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                            IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                            string relationshipId = sheets.First().Id.Value;
                            WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                            Worksheet workSheet = worksheetPart.Worksheet;
                            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                            IEnumerable<Row> rows = sheetData.Descendants<Row>();

                            RowViewModels.Clear();

                            foreach (Row row in rows)
                            {
                                RowViewModel mModel = new RowViewModel();
                                //TODO fix hacky limit
                                for (int i = 1; i < row.Descendants<Cell>().Count() && i < 100; i++)
                                {
                                    mModel.values[i - 1] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i - 1));
                                }

                                RowViewModels.Add(mModel);

                            }

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
                                //TODO fix hacky limit
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
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to open the file.  Is the target file open or in use?");
                    Console.WriteLine(e.ToString());
                }

                pd.Close();

            }
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[System.Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
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
