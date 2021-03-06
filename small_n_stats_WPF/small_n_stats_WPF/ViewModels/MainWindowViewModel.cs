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

 Demand Calculator utilizes Reogrid to leverage to load, save, and display data

    Reogrid is distributed under this license:

    MIT License
    
    Copyright(c) 2013-2016 Jing<lujing at unvell.com>
    Copyright(c) 2013-2016 unvell.com, All rights reserved.
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
    
    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

*/

using Microsoft.Win32;
using RDotNet;
using small_n_stats_WPF.Dialogs;
using small_n_stats_WPF.Utilities;
using small_n_stats_WPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace small_n_stats_WPF.ViewModels
{
    class MainWindowViewModel : BaseViewModel
    {
        #region Observable Bindings

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

        public RelayCommand FileCutCommand { get; set; }
        public RelayCommand FileCopyCommand { get; set; }
        public RelayCommand FilePasteCommand { get; set; }
        public RelayCommand FilePasteInvertCommand { get; set; }
        public RelayCommand FileUndoCommand { get; set; }
        public RelayCommand FileRedoCommand { get; set; }

        public RelayCommand FileSaveAsCommand { get; set; }
        public RelayCommand FileCloseCommand { get; set; }
        public RelayCommand FileSaveNoDialogCommand { get; set; }
        public RelayCommand RecentsClearCommand { get; set; }
        public RelayCommand HelpCommand { get; set; }

        public RelayCommand AddCommand { get; set; }
        public RelayCommand RenameCommand { get; set; }
        public RelayCommand ResizeCommand { get; set; }
        public RelayCommand RemoveSheetCommand { get; set; }

        public RelayCommand ViewLoadedCommand { get; set; }
        public RelayCommand ViewClosingCommand { get; set; }

        /* Menu Items */

        public RelayCommand UnifiedDemandCurveWindowCommand { get; set; }
        public RelayCommand InformationWindowCommand { get; set; }

        REngine engine;

        public RelayCommand RDotNetLicenseWindowCommand { get; set; }
        public RelayCommand NlmrtLicenseWindowCommand { get; set; }
        public RelayCommand NlstoolsLicenseWindowCommand { get; set; }
        public RelayCommand RLicenseWindowCommand { get; set; }
        public RelayCommand ReogridLicenseWindowCommand { get; set; }
        public RelayCommand BeezdemandLicenseWindowCommand { get; set; }
        public RelayCommand DevtoolsLicenseWindowCommand { get; set; }
        public RelayCommand DigestLicenseWindowCommand { get; set; }
        public RelayCommand LicenseWindowCommand { get; set; }

        /* End Menu Items */

        /* Misc Commands */

        public RelayCommand SaveLogsWindowCommand { get; set; }
        public RelayCommand ClearLogsWindowCommand { get; set; }
        //public RelayCommand DeleteSelectedCommand { get; set; }
        //public RelayCommand CutSelectedCommand { get; set; }
        //public RelayCommand PasteInvertedCommand { get; set; }

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

            FileUndoCommand = new RelayCommand(param => App.Workbook.Undo(), param => true);
            FileRedoCommand = new RelayCommand(param => App.Workbook.Redo(), param => true);
            FileCutCommand = new RelayCommand(param => App.Workbook.CurrentWorksheet.Cut(), param => true);
            FileCopyCommand = new RelayCommand(param => App.Workbook.CurrentWorksheet.Copy(), param => true);
            FilePasteCommand = new RelayCommand(param => App.Workbook.CurrentWorksheet.Paste(), param => true);
            FilePasteInvertCommand = new RelayCommand(param => PasteInverted(), param => true);

            FileSaveNoDialogCommand = new RelayCommand(param => SaveFileWithoutDialog(), param => true);
            FileOpenNoDialogCommand = new RelayCommand(param => FileOpenNoDialog(param), param => true);

            AddCommand = new RelayCommand(param => AddSheet(), param => true);
            ResizeCommand = new RelayCommand(param => ResizeCurrentSheet(), param => true);
            RenameCommand = new RelayCommand(param => RenameSheet(), param => true);
            RemoveSheetCommand = new RelayCommand(param => DeleteCurrentSheet(), param => true);

            HelpCommand = new RelayCommand(param => OpenHelpWindow(), param => true);

            RecentsClearCommand = new RelayCommand(param => ClearRecents(), param => true);
            
            RecentStuff = new ObservableCollection<MenuItem>();

            recentsArray = Properties.Settings.Default.RecentFiles.Trim().Split(';');

            BitmapImage mIcon = new BitmapImage(new Uri("pack://application:,,,/Resources/Textfile_818_16x.png"));

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
                        CommandParameter = recentFileLocation,
                        Icon = new Image
                        {
                            Source = mIcon
                        }
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

            #region TriggerCommands

            ViewLoadedCommand = new RelayCommand(param => ViewLoaded(), param => true);
            ViewClosingCommand = new RelayCommand(param => ViewClosed(), param => true);

            #endregion

            #region UICommands

            UnifiedDemandCurveWindowCommand = new RelayCommand(param => OpenUnifiedDemandCurveWindow(), param => true);
            InformationWindowCommand = new RelayCommand(param => OpenInformationWindow(), param => true);

            #endregion

            #region LicenseCommands
            
            RDotNetLicenseWindowCommand = new RelayCommand(param => RdotNetLicenseInformationWindow(), param => true);
            NlmrtLicenseWindowCommand = new RelayCommand(param => NlmrtLicenseInformationWindow(), param => true);
            NlstoolsLicenseWindowCommand = new RelayCommand(param => NlsToolsLicenseInformationWindow(), param => true);
            RLicenseWindowCommand = new RelayCommand(param => RLicenseInformationWindow(), param => true);
            ReogridLicenseWindowCommand = new RelayCommand(param => ReogridLicenseWindow(), param => true);
            BeezdemandLicenseWindowCommand = new RelayCommand(param => BeezdemandLicenseInformationWindow(), param => true);

            DevtoolsLicenseWindowCommand = new RelayCommand(param => DevtoolsLicenseInformationWindow(), param => true);
            DigestLicenseWindowCommand = new RelayCommand(param => DigestLicenseInformationWindow(), param => true);

            LicenseWindowCommand = new RelayCommand(param => LicenseInformationWindow(), param => true);

            #endregion

            #region Context Menu

            var mContextMenu = new ContextMenu();
            mContextMenu.Items.Add(new MenuItem
            {
                Header = "Cut",
                Command = FileCutCommand
            });
            mContextMenu.Items.Add(new MenuItem
            {
                Header = "Copy",
                Command = FileCopyCommand
            });
            mContextMenu.Items.Add(new MenuItem
            {
                Header = "Paste",
                Command = FilePasteCommand
            });
            mContextMenu.Items.Add(new MenuItem
            {
                Header = "Paste Inverted",
                Command = FilePasteInvertCommand
            });

            App.Workbook.CellsContextMenu = mContextMenu;

            App.Workbook.SheetTabNewButtonVisible = false;

            #endregion

        }

        #region UI

        /// <summary>
        /// Add new sheet into workbook
        /// </summary>
        private void AddSheet()
        {
            var addNewSheet = new NamingDialog();
            addNewSheet.Title = "Please name the new sheet";

            addNewSheet.ShowDialog();

            string mEntry = addNewSheet.nameBox.Text;

            if (mEntry == null || mEntry.Trim().Length == 0)
            {
                MessageBox.Show("Invalid name");

                return;
            }
            else
            {
                string mName = new string(mEntry.Take(24).ToArray());

                if (!App.Workbook.Worksheets.Any(s => s.Name.Contains(mName)))
                {
                    var sheet = App.Workbook.CreateWorksheet(mName);
                    App.Workbook.AddWorksheet(sheet);
                }
                else
                {
                    var sheet = App.Workbook.CreateWorksheet(mName + "1");
                    App.Workbook.AddWorksheet(sheet);
                }
            }
        }

        /// <summary>
        /// Rename the current sheet
        /// </summary>
        private void RenameSheet()
        {
            var renameNewSheet = new NamingDialog();
            renameNewSheet.Title = "Rename the current sheet";

            renameNewSheet.ShowDialog();

            string mEntry = renameNewSheet.nameBox.Text;

            if (mEntry == null || mEntry.Trim().Length == 0)
            {
                MessageBox.Show("Invalid name");

                return;
            }
            else
            {
                string mName = new string(mEntry.Take(24).ToArray());
                App.Workbook.CurrentWorksheet.Name = mName;
            }
        }

        /// <summary>
        /// Calls to RG to increase or decrease cells
        /// </summary>
        private void ResizeCurrentSheet()
        {
            var getNewSizes = new ResizeDialog();
            getNewSizes.rowBox.Text = App.Workbook.CurrentWorksheet.Rows.ToString();
            getNewSizes.colBox.Text = App.Workbook.CurrentWorksheet.Columns.ToString();

            getNewSizes.ShowDialog();

            if (getNewSizes.rowBox.Text != "" && getNewSizes.colBox.Text != "")
            {
                App.Workbook.CurrentWorksheet.Resize(int.Parse(getNewSizes.rowBox.Text), int.Parse(getNewSizes.colBox.Text));
            }
        }

        /// <summary>
        /// Calls to RG to delete the currently selected sheet
        /// </summary>
        private void DeleteCurrentSheet()
        {
            if (App.Workbook.Worksheets.Count == 1)
            {
                MessageBox.Show("Only one sheet remains. All workbooks must have at least 1 sheet.");

                return;
            }

            var confirmDelete = new YesNoDialog();
            confirmDelete.Title = "Confirm Delete";
            confirmDelete.QuestionText = "Are you sure you want to delete this sheet?";

            confirmDelete.ShowDialog();

            if (confirmDelete.ReturnedAnswer)
            {
                App.Workbook.RemoveWorksheet(App.Workbook.CurrentWorksheet);
            }
        }

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
            var pathDir = Path.GetDirectoryName(filePath);
            Properties.Settings.Default.LastDirectory = pathDir;

            recentsArray = Properties.Settings.Default.RecentFiles.Split(';');

            List<string> workingRecents = recentsArray.Select(item => item).Where(item => item.Trim().Length > 1).ToList();

            if (!workingRecents.Contains(filePath))
            {
                workingRecents.Insert(0, filePath);
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
        /// Transposition a-la matrix, but list of arrays
        /// </summary>
        /// <param name="arrayList"></param>
        /// <returns></returns>
        static List<string[]> CreateTransposedList(List<string[]> arrayList)
        {
            int lengthTemp = arrayList[0].Length;
            string[,] tempMatrix = new string[arrayList.Count, lengthTemp];

            for (int i = 0; i < arrayList.Count; i++)
            {
                string[] tempArray = arrayList[i];

                if (tempArray.Length != lengthTemp)
                {
                    return null;
                }
                for (int j = 0; j < lengthTemp; j++)
                {
                    tempMatrix[i, j] = tempArray[j];
                }
            }

            string[,] transposedMatrix = new string[tempMatrix.GetLength(1), tempMatrix.GetLength(0)];
            for (int i = 0; i < tempMatrix.GetLength(1); i++)
            {
                for (int j = 0; j < tempMatrix.GetLength(0); j++)
                {
                    transposedMatrix[i, j] = tempMatrix[j, i];
                }
            }

            List<string[]> returnList = new List<string[]>();

            string[] holder;
            for (int i = 0; i < transposedMatrix.GetLength(0); i++)
            {
                holder = new string[transposedMatrix.GetLength(1)];
                for (int j = 0; j < transposedMatrix.GetLength(1); j++)
                {
                    holder[j] = transposedMatrix[i, j];
                }
                returnList.Add(holder);
            }

            return returnList;
        }

        #endregion

        #region Triggers

        /// <summary>
        /// Loaded event trigger
        /// </summary>
        private void ViewLoaded()
        {
            IntroWindow introWindow = new IntroWindow();
            introWindow.Owner = App.ApplicationWindow;
            introWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            introWindow.Show();

            bool failed = false;

            SendMessageToOutput("Welcome to Demand Curve Calculator!");
            SendMessageToOutput("");
            SendMessageToOutput("All view elements loaded");
            SendMessageToOutput("");

            StringReader licenseFile = new StringReader(Properties.Resources.LICENSE);

            string line;

            while ((line = licenseFile.ReadLine()) != null)
            {
                SendMessageToOutput(line);
            }

            SendMessageToOutput("Loading R interop libraries (R.NET Community)");

            try
            {
                introWindow.checkNet.Foreground = Brushes.Green;

                REngine.SetEnvironmentVariables();

                SendMessageToOutput("Attempting to link with R installation.");

                engine = REngine.GetInstance();

                SendMessageToOutput("Attempting to Load core binaries...");

                engine.Initialize();
                engine.AutoPrint = false;

                introWindow.checkR.Foreground = Brushes.Green;
                introWindow.checkR2.Foreground = Brushes.Green;

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
                    
                    introWindow.loadText.Text = "Loading R Packages";

                    bool loadedDevTools = engine.Evaluate("require(devtools)").AsLogical().First();

                    if (loadedDevTools)
                    {
                        introWindow.checkDevtools.Foreground = Brushes.Green;
                    }
                    else
                    {
                        SendMessageToOutput("Attempting to install devtools packages for first time!");
                        introWindow.loadText.Text = "Downloading devtools...";
                        engine.Evaluate("if (!require(devtools)) { install.packages('devtools', repos = 'http://cran.us.r-project.org') }");

                        loadedDevTools = engine.Evaluate("require(devtools)").AsLogical().First();

                        if (loadedDevTools)
                        {
                            introWindow.checkDevtools.Foreground = Brushes.Green;
                        }
                    }

                    introWindow.loadText.Text = "Loading R Packages";

                    bool loadedDigest = engine.Evaluate("require(digest)").AsLogical().First();

                    if (loadedDigest)
                    {
                        introWindow.checkDigest.Foreground = Brushes.Green;
                    }
                    else
                    {
                        SendMessageToOutput("Attempting to install digest packages for first time!");
                        introWindow.loadText.Text = "Downloading digest...";
                        engine.Evaluate("if (!require(digest)) { install.packages('digest', repos = 'http://cran.us.r-project.org') }");

                        loadedDigest = engine.Evaluate("require(digest)").AsLogical().First();

                        if (loadedDigest)
                        {
                            introWindow.checkDigest.Foreground = Brushes.Green;
                        }
                    }

                    introWindow.loadText.Text = "Loading R Packages";

                    bool loadedRepository = engine.Evaluate("require(beezdemand)").AsLogical().First();

                    if (loadedRepository)
                    {
                        // Update as needed
                        //package_deps("rNVD3", repos = "ramnathv/rNVD3")
                        //engine.Evaluate("devtools::update_packages('beezdemand')");
                        engine.Evaluate("devtools::package_deps('beezdemand', repos='miyamot0/beezdemand')");
                        engine.Evaluate("devtools::update_packages('beezdemand')");

                        introWindow.checkBeezdemand.Foreground = Brushes.Green;
                    }
                    else
                    {
                        SendMessageToOutput("Attempting to install beezdemand packages for first time!");
                        introWindow.loadText.Text = "Downloading beezdemand...";

                        engine.Evaluate("devtools::package_deps('beezdemand', repos='miyamot0/beezdemand')");
                        engine.Evaluate("devtools::update_packages('beezdemand')");
                        //engine.Evaluate("if (!require(beezdemand)) { devtools::install_github('miyamot0/beezdemand') }");

                        loadedRepository = engine.Evaluate("require(beezdemand)").AsLogical().First();

                        if (loadedRepository)
                        {
                            // Update as needed
                            engine.Evaluate("devtools::package_deps('beezdemand', repos='miyamot0/beezdemand')");
                            engine.Evaluate("devtools::update_packages('beezdemand')");

                            introWindow.checkBeezdemand.Foreground = Brushes.Green;
                        }
                    }

                    introWindow.loadText.Text = "Loading R Packages";

                    bool loadedNlmrt = engine.Evaluate("require(nlmrt)").AsLogical().First();

                    if (loadedNlmrt)
                    {
                        introWindow.checkNlmrt.Foreground = Brushes.Green;
                    }
                    else
                    {
                        SendMessageToOutput("Attempting to install nlmrt packages for first time!");
                        introWindow.loadText.Text = "Downloading nlmrt...";
                        engine.Evaluate("if (!require(nlmrt)) { install.packages('nlmrt', repos = 'http://cran.us.r-project.org') }");

                        loadedNlmrt = engine.Evaluate("require(nlmrt)").AsLogical().First();

                        if (loadedNlmrt)
                        {
                            introWindow.checkNlmrt.Foreground = Brushes.Green;
                        }
                    }

                    introWindow.loadText.Text = "Loading R Packages";

                    bool loadedNlstools = engine.Evaluate("require(nlstools)").AsLogical().First();

                    if (loadedNlstools)
                    {
                        introWindow.checkNlstools.Foreground = Brushes.Green;
                    }
                    else
                    {
                        SendMessageToOutput("Attempting to install nlstools packages for first time!");
                        introWindow.loadText.Text = "Downloading nlstools...";
                        engine.Evaluate("if (!require(nlstools)) { install.packages('nlstools', repos = 'http://cran.us.r-project.org') }");

                        loadedNlstools = engine.Evaluate("require(nlstools)").AsLogical().First();

                        if (loadedNlstools)
                        {
                            introWindow.checkNlstools.Foreground = Brushes.Green;
                        }
                    }
                    
                    if (loadedNlstools && loadedNlmrt && !failed)
                    {
                        introWindow.loadText.Text = "All necessary components found!";
                        introWindow.loadText.Foreground = Brushes.Green;
                        SendMessageToOutput("All required packages have been found.  Ready to proceed.");
                    }
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
                SendMessageToOutput("Citation:: " + string.Join("", engine.Evaluate("citation()$textVersion").AsCharacter().ToArray()));
                SendMessageToOutput("");
                SendMessageToOutput("nlmrt R Package - GPLv2 Licensed. Copyright (C) 2016. John C. Nash.");
                SendMessageToOutput("Citation:: " + string.Join("", engine.Evaluate("citation('nlmrt')$textVersion").AsCharacter().ToArray()));
                SendMessageToOutput("");
                SendMessageToOutput("nlstools R Package - GPLv2 Licensed. Copyright(C) 2015 Florent Baty and Marie-Laure Delignette - Muller, with contributions from Sandrine Charles, Jean - Pierre Flandrois, and Christian Ritz.");
                SendMessageToOutput("Citation:: " + string.Join("", engine.Evaluate("citation('nlstools')$textVersion").AsCharacter().ToArray()));
                SendMessageToOutput("");
                SendMessageToOutput("beezdemand R Package - GPLv2+ Licensed. Copyright (c) 2015, Brent Kaplan.");
                SendMessageToOutput("Citation:: " + string.Join("", engine.Evaluate("citation('beezdemand')$textVersion").AsCharacter().ToArray()));
                SendMessageToOutput("");
                SendMessageToOutput("Reogrid - MIT Licensed. Copyright(c) 2013-2016 Jing {lujing at unvell.com}, Copyright(c) 2013-2016 unvell.com. ");
                SendMessageToOutput("RdotNet: Interface for the R Statistical Package - New BSD License (BSD 2-Clause). Copyright(c) 2010, RecycleBin. All rights reserved.");
                SendMessageToOutput("SharpVectors: Library for rendering SVG - New BSD License (BSD 3-Clause). Copyright(c) 2010, SharpVectorGraphics. All rights reserved.");
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

        /// <summary>
        /// Custom paste operation, swapping V/H loopings to make a transposition
        /// </summary>
        private void PasteInverted()
        {
            List<string[]> rowData = ClipboardTools.ReadAndParseClipboardData();

            if (rowData == null)
            {
                return;
            }

            int lowRow = App.Workbook.CurrentWorksheet.FocusPos.Row,        // Current highlighted cell's row
                highRow = App.Workbook.CurrentWorksheet.Rows,               // Highest row in table
                lowCol = App.Workbook.CurrentWorksheet.FocusPos.Col,        // Current highlighted cell's column
                highCol = App.Workbook.CurrentWorksheet.Columns,
                pasteContentRowIterator = 0,
                pasteContentColumnIterator = 0;

            try
            {
                rowData = CreateTransposedList(rowData);
            }
            catch
            {
                // Error in constructing dimensions, fail out
                return;
            }

            if (rowData == null) return;

            for (int i = lowRow; (i <= highRow) && (pasteContentRowIterator < rowData.Count); i++)
            {
                if (i == highRow)
                {
                    App.Workbook.CurrentWorksheet.AppendRows(1);
                    highRow = (pasteContentRowIterator + 1 < rowData.Count) ? highRow + 1 : highRow;
                }

                pasteContentColumnIterator = 0;

                for (int j = lowCol; pasteContentColumnIterator < rowData[pasteContentRowIterator].Length; j++)
                {
                    if (j == highCol)
                    {
                        App.Workbook.CurrentWorksheet.AppendCols(1);
                        highCol = (pasteContentColumnIterator + 1 < rowData[0].Length) ? highCol + 1 : highCol;
                    }

                    App.Workbook.CurrentWorksheet.CreateAndGetCell(i, j).Data = rowData[pasteContentRowIterator][pasteContentColumnIterator];
                    pasteContentColumnIterator++;
                }

                pasteContentRowIterator++;
            }
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
            window.Owner = App.ApplicationWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void ReogridLicenseWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Reogrid",
                licenseText = Properties.Resources.License_Reogrid
            };
            window.Owner = App.ApplicationWindow;
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
            window.Owner = App.ApplicationWindow;
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
            window.Owner = App.ApplicationWindow;
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
            window.Owner = App.ApplicationWindow;
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
            window.Owner = App.ApplicationWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void DigestLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Digest",
                licenseText = Properties.Resources.License_Digest
            };
            window.Owner = App.ApplicationWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        /// <summary>
        /// License window
        /// </summary>
        private void DevtoolsLicenseInformationWindow()
        {
            var window = new License();
            window.DataContext = new LicenseViewModel
            {
                licenseTitle = "License - Devtools",
                licenseText = Properties.Resources.License_Devtools
            };
            window.Owner = App.ApplicationWindow;
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
            window.Owner = App.ApplicationWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Show();
        }

        #endregion Licenses

        #region OpenWindows

        /// <summary>
        /// Single mode analysis window
        /// </summary>
        private void OpenUnifiedDemandCurveWindow()
        {
            var mWin = new DemandCurveUnifiedWindow();
            mWin.Owner = App.ApplicationWindow;
            mWin.windowTitle.Text = "Demand Curve Analysis";
            mWin.DataContext = new UnifiedDemandCurveViewModel
            {
                mWindow = App.ApplicationWindow,
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
            mWin.Owner = App.ApplicationWindow;
            mWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mWin.Show();
        }

        /// <summary>
        /// Help window
        /// </summary>
        private void OpenHelpWindow()
        {
            var mWin = new HelpWindow();
            mWin.Owner = App.ApplicationWindow;
            mWin.Show();
        }

        #endregion

        #region FileIO

        /// <summary>
        /// Creates new spreadsheet, not really "file"
        /// </summary>
        private void CreateNewFile()
        {
            UpdateTitle("New File");
            workingSheet = "Sheet1";

            haveFileLoaded = false;
        }

        /// <summary>
        /// Saves file, usually from Ctrl+S binding
        /// </summary>
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
                saveFileDialog1.Filter = "Excel file (*.xlsx)|*.xlsx|CSV file (*.csv)|*.csv|All files (*.*)|*.*";

                if (saveFileDialog1.ShowDialog() == true)
                {
                    try
                    {
                        string mExt = Path.GetExtension(saveFileDialog1.FileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        if (mExt.Equals(".xlsx"))
                        {
                            App.Workbook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
                        }
                        else if (mExt.Equals(".csv"))
                        {
                            App.Workbook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.CSV);
                        }
                        else
                        {
                            return;
                        }

                        UpdateTitle(saveFileDialog1.SafeFileName);

                        path = Path.GetDirectoryName(saveFileDialog1.FileName);

                        haveFileLoaded = true;

                        AddToRecents(@saveFileDialog1.FileName);


                        SendMessageToOutput("Saved: " + @saveFileDialog1.FileName);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                        SendMessageToOutput("Error: " + e.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Saves file with a dialog 
        /// </summary>
        private void SaveFileAs()
        {
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
                        App.Workbook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.Excel2007);
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        App.Workbook.Save(saveFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat.CSV);
                    }
                    else
                    {
                        return;
                    }

                    workingSheet = "Model Selector";

                    UpdateTitle(saveFileDialog1.SafeFileName);

                    path = Path.GetDirectoryName(saveFileDialog1.FileName);

                    haveFileLoaded = true;

                    AddToRecents(@saveFileDialog1.FileName);

                    SendMessageToOutput("Saved: " + @saveFileDialog1.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                    SendMessageToOutput("Error: " + e.ToString());
                    haveFileLoaded = false;
                }
            }
        }

        /// <summary>
        /// Saves file without a dialog call
        /// </summary>
        private void SaveFileWithoutDialog()
        {
            if (haveFileLoaded)
            {
                try
                {
                    string mExt = Path.GetExtension(Path.Combine(path, title));

                    path = Path.GetDirectoryName(Path.Combine(path, title));

                    if (mExt.Equals(".xlsx"))
                    {
                        App.Workbook.Save(Path.Combine(path, title), unvell.ReoGrid.IO.FileFormat.Excel2007);
                    }
                    else if (mExt.Equals(".csv"))
                    {
                        App.Workbook.Save(Path.Combine(path, title), unvell.ReoGrid.IO.FileFormat.CSV);
                    }
                    else
                    {
                        return;
                    }

                    UpdateTitle(title);

                    SendMessageToOutput("Saved: " + Path.Combine(path, title));
                }
                catch (Exception e)
                {
                    MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                    SendMessageToOutput("Error: " + e.ToString());
                }
            }
        }

        /// <summary>
        /// Opens a file with a dialog
        /// </summary>
        private void OpenFile()
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                if (Directory.Exists(Properties.Settings.Default.LastDirectory))
                {
                    openFileDialog1.InitialDirectory = Properties.Settings.Default.LastDirectory;
                }
                else
                {
                    openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                openFileDialog1.Filter = "Spreadsheet Files (XLSX, CSV)|*.xlsx;*.csv";
                openFileDialog1.Title = "Select a spreadsheet";

                if (openFileDialog1.ShowDialog() == true)
                {
                    string mExt = Path.GetExtension(openFileDialog1.FileName);
                    path = Path.GetDirectoryName(openFileDialog1.FileName);

                    try
                    {
                        if (mExt.Equals(".xlsx"))
                        {
                            App.Workbook.Load(openFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat._Auto);

                            workingSheet = openFileDialog1.SafeFileName;
                            UpdateTitle(openFileDialog1.SafeFileName);
                            haveFileLoaded = true;
                        }
                        else if (mExt.Equals(".csv"))
                        {
                            App.Workbook.Load(openFileDialog1.FileName, unvell.ReoGrid.IO.FileFormat._Auto);

                            workingSheet = openFileDialog1.SafeFileName;
                            UpdateTitle(openFileDialog1.SafeFileName);
                            haveFileLoaded = true;
                        }
                        else
                        {
                            return;
                        }

                        if (workingSheet != string.Empty)
                        {
                            AddToRecents(@openFileDialog1.FileName);
                        }
                        else
                        {
                            title = "Discounting Model Selection - New File";
                        }

                        SendMessageToOutput("Opened: " + @openFileDialog1.FileName);
                    }
                    catch (IOException e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                        SendMessageToOutput("Error: " + e.ToString());
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("We weren't able to save.  Is the target file either open, missing or in use?");
                        SendMessageToOutput("Error: " + e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                SendMessageToOutput("Error: " + e.ToString());
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
            string mExt = Path.GetExtension(@filePath);

            path = Path.GetDirectoryName(@filePath);

            try
            {
                if (mExt.Equals(".xlsx"))
                {
                    App.Workbook.Load(@filePath, unvell.ReoGrid.IO.FileFormat._Auto);

                    UpdateTitle(Path.GetFileName(@filePath));
                    haveFileLoaded = true;
                }
                else if (mExt.Equals(".csv"))
                {
                    App.Workbook.Load(@filePath, unvell.ReoGrid.IO.FileFormat._Auto);

                    UpdateTitle(Path.GetFileName(filePath));
                    haveFileLoaded = true;
                }
                else
                {
                    return;
                }

                AddToRecents(@filePath);

                SendMessageToOutput("Opened: " + @filePath);
            }
            catch (IOException e)
            {
                SendMessageToOutput("Error: " + e.ToString());
                MessageBox.Show("We weren't able to open the file.  Is the target file either open, missing or in use?");
            }
            catch (Exception e)
            {
                SendMessageToOutput("Error: " + e.ToString());
                MessageBox.Show("We weren't able to open the file.  Is the target file either open, missing or in use?");
            }
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
                var item = RecentStuff.Where(r => r.Header.ToString() == path).FirstOrDefault();

                RecentStuff.Remove(item);
                RecentStuff.Insert(0, item);

                OpenFileNoDialog(path);
            }
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
            App.ApplicationWindow.OutputEvents(message);
        }

        private void SaveLogs()
        {
            App.ApplicationWindow.SaveLogs();
        }

        private void ClearLogs()
        {
            App.ApplicationWindow.ClearLogs();
        }

        #endregion Logging

    }
}
