/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Interfaces
{
    public interface OutputWindowInterface
    {
        void SendMessageToOutput(string message);
    }
}
