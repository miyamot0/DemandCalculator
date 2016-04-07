namespace small_n_stats_WPF.Models
{
    class TauAModel
    {
        public class Tau
        {
            public double ConfidenceLevel { get; set; }
            public double StandardError { get; set; }
            public double LowerBound { get; set; }
            public double TauA { get; set; }
            public double UpperBound { get; set; }
        }

        public Tau TAU80 { get; set; }
        public Tau TAU85 { get; set; }
        public Tau TAU90 { get; set; }
        public Tau TAU95 { get; set; }
    }
}
