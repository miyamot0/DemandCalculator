﻿/* 
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
        /// Koffarnus' exponentiated function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentiatedDemandFunction()
        {
            return Properties.Resources.ExponentiatedDemandFunctions;
        }
    }
}
