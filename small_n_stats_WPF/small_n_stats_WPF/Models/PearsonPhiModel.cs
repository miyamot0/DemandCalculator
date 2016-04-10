/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Models
{
    class PearsonPhiModel
    {
        public class Phi
        {
            public double ConfidenceLevel { get; set; }
            public double LowerBound { get; set; }
            public double PearsonPhi { get; set; }
            public double UpperBound { get; set; }
        }

        public Phi Phi80 { get; set; }
        public Phi Phi85 { get; set; }
        public Phi Phi90 { get; set; }
        public Phi Phi95 { get; set; }
    }
}
