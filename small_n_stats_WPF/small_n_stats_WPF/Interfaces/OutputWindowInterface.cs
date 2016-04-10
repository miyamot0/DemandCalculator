/*
 * Shawn Gilroy, Copyright 2016. Licensed under GPL-2.
 * Small n Stats Application
 * Modeled from conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Interfaces
{
    public interface OutputWindowInterface
    {
        void SendMessageToOutput(string message);
    }
}
