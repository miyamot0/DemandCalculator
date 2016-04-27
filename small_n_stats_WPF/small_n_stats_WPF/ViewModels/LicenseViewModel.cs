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

namespace small_n_stats_WPF.ViewModels
{
    class LicenseViewModel : BaseViewModel
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

        public LicenseViewModel() { }
    }
}
