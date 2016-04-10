/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * Small n Stats Application
 * Modeled from conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Models
{
    class PercentExceedingMedianModel
    {
        public class PEM
        {
            public double ConfidenceLevel { get; set; }
            public double StandardError { get; set; }
            public double LowerBound { get; set; }
            public double PercentExceedingMedian { get; set; }
            public double UpperBound { get; set; }
        }

        public PEM PEM80 { get; set; }
        public PEM PEM85 { get; set; }
        public PEM PEM90 { get; set; }
        public PEM PEM95 { get; set; }
    }
}
