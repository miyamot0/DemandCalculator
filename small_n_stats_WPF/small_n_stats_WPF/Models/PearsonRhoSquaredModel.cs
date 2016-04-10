﻿/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * Small n Stats Application
 * Modeled from conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Models
{
    class PearsonRhoSquaredModel
    {
        public class RhoSquared
        {
            public double ConfidenceLevel { get; set; }
            public double LowerBound { get; set; }
            public double PearsonRhoSquared { get; set; }
            public double UpperBound { get; set; }
        }

        public RhoSquared RhoSquared80 { get; set; }
        public RhoSquared RhoSquared85 { get; set; }
        public RhoSquared RhoSquared90 { get; set; }
        public RhoSquared RhoSquared95 { get; set; }
    }
}
