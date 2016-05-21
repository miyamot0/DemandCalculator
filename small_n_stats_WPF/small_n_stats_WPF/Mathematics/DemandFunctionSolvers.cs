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

namespace small_n_stats_WPF.Mathematics
{
    class DemandFunctionSolvers
    {
        /// <summary>
        /// Hursh & Silverburg's exponetial function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentialDemandFunction()
        {
            return Properties.Resources.ExponentialDemandFunctions;
        }

        /// <summary>
        /// Hursh & Silverburg's exponetial function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentialDemandFunctionKFittings()
        {
            return Properties.Resources.ExponentialDemandFunctionsKFitted;
        }

        /// <summary>
        /// Koffarnus' exponentiated function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentiatedDemandFunction()
        {
            return Properties.Resources.ExponentiatedDemandFunctions;
        }

        /// <summary>
        /// Koffarnus' exponentiated function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentiatedDemandFunctionKFittings()
        {
            return Properties.Resources.ExponentiatedDemandFunctionsKFitted;
        }

        /// <summary>
        /// Graphing code for logged exponential model
        /// </summary>
        public static string GetExponentialGraphingFunction()
        {
            return Properties.Resources.GraphingExponential;
        }

        /// <summary>
        /// Graphing code for logged exponentiated model
        /// </summary>
        public static string GetExponentiatedGraphingFunction()
        {
            return Properties.Resources.GraphingExponentiated;
        }

        /// <summary>
        /// Stein check for systematic purchase data
        /// Original Author: Brent Kaplan
        /// </summary>
        public static string GetSteinSystematicCheck()
        {
            return Properties.Resources.SteinSystematicCheck;
        }
    }
}
