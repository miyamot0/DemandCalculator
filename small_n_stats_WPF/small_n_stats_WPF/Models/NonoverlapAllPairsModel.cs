namespace small_n_stats_WPF.Models
{
    class NonoverlapAllPairsModel
    {
        public class NAP
        {
            public double ConfidenceLevel { get; set; }
            public double StandardError { get; set; }
            public double LowerBound { get; set; }
            public double NonoverlapAllPairs { get; set; }
            public double UpperBound { get; set; }
        }

        public NAP NAP80 { get; set; }
        public NAP NAP85 { get; set; }
        public NAP NAP90 { get; set; }
        public NAP NAP95 { get; set; }
    }
}
