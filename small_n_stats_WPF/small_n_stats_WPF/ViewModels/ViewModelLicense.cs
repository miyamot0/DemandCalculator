﻿/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * View Model for re-displaying licenses from referenced works
 * 
 */

namespace small_n_stats_WPF.ViewModels
{
    class ViewModelLicense : ViewModelBase
    {
        public string licenseTitle { get; set; }
        public string LicenseTitle
        {
            get { return licenseTitle; }
            set
            {
                licenseTitle = value;
                OnPropertyChanged("LicenseTitle");
            }
        }

        public string licenseText { get; set; }
        public string LicenseText
        {
            get { return licenseText; }
            set
            {
                licenseText = value;
                OnPropertyChanged("LicenseText");
            }
        }

        public ViewModelLicense() { }
    }
}
