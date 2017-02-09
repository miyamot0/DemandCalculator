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

*/

namespace small_n_stats_WPF.Tags
{
    static class Conventions
    {
        #region R Data Objects

        /// <summary>
        /// Name for the constructed dataframe
        /// </summary>
        public static string NamedDataFrame = "dat";

        /// <summary>
        /// 
        /// </summary>
        public static string DescriptiveDataFrame = "descriptiveMetrics";

        /// <summary>
        /// 
        /// </summary>
        public static string SteinDataFrame = "steinMetrics";

        /// <summary>
        /// 
        /// </summary>
        public static string FittedDataFrame = "fittedFrame";

        #endregion

        #region R Logic

        /// <summary>
        /// Tag for R TRUE logic
        /// </summary>
        public static string RTrue = "TRUE";

        /// <summary>
        /// Tag for R FALSE logic
        /// </summary>
        public static string RFalse = "FALSE";

        /// <summary>
        /// Tag for R NULL logic
        /// </summary>
        public static string RNull = "NULL";

        #endregion

        #region Model Tags

        /// <summary>
        /// Name for Exponential Model
        /// </summary>
        public static string ExponentialModel = "'hs'";

        /// <summary>
        /// Name for Exponentiated Model
        /// </summary>
        public static string ExponentiatedModel = "'koff'";

        /// <summary>
        /// Name for Linear Model
        /// </summary>
        public static string LinearModel = "'linear'";

        #endregion

        #region K Tags

        /// <summary>
        /// Fit K
        /// </summary>
        public static string FitK = "'fit'";

        /// <summary>
        /// Construct individualized k
        /// </summary>
        public static string IndividualK = "'ind'";

        /// <summary>
        /// Construct aggregated k
        /// </summary>
        public static string ShareK = "'share'";

        #endregion
    }
}
