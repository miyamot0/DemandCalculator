/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Models
{
    class PearsonRhoModel
    {
        public class Rho
        {
            public double ConfidenceLevel { get; set; }
            public double LowerBound { get; set; }
            public double PearsonRho { get; set; }
            public double UpperBound { get; set; }
        }

        public Rho Rho80 { get; set; }
        public Rho Rho85 { get; set; }
        public Rho Rho90 { get; set; }
        public Rho Rho95 { get; set; }
    }
}
