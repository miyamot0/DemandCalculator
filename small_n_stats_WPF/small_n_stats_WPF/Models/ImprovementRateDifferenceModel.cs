/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Models
{
    class ImprovementRateDifferenceModel
    {
        public class IRD
        {
            public double ConfidenceLevel { get; set; }
            public double LowerBound { get; set; }
            public double ImprovementRateDifference { get; set; }
            public double UpperBound { get; set; }
        }

        public IRD IRD80 { get; set; }
        public IRD IRD85 { get; set; }
        public IRD IRD90 { get; set; }
        public IRD IRD95 { get; set; }
    }
}
