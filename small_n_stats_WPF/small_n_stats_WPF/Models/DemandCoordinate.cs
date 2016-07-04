namespace small_n_stats_WPF.Models
{
    public class DemandCoordinate
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double P { get; set; }
        public double Expend { get; set; }

        public DemandCoordinate()
        {

        }

        public DemandCoordinate MakeDemandCoordinate(double x, double y, double p)
        {
            return new DemandCoordinate { X = x, Y = y, P = p, Expend = (x * y) };
        }
    }
}
