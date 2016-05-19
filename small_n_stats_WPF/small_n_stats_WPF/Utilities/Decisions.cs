namespace small_n_stats_WPF.Utilities
{
    public enum YValueDecisions
    {
        DoNothing,
        DropZeros,
        ChangeHundredth,
        OnePercentLowest
    }

    public enum XValueDecisions
    {
        DoNothing,
        ChangeHundredth,
        DropZeros
    }

    public enum KValueDecisions
    {
        DeriveValues,
        UseSuppliedValues
    }
}
