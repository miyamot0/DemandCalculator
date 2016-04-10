/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

namespace small_n_stats_WPF.Interfaces
{
    public interface SpreadsheetInterface
    {
        void UpdateTitle(string Title);

        void GainFocus();

        bool NewFile();

        void SaveFile(string path, string title);

        string SaveFileWithDialog(string title);

        string SaveFileAs(string title);

        string[] OpenFile();

        void ShutDown();
    }
}
