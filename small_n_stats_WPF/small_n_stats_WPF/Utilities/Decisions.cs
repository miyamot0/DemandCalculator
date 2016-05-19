namespace small_n_stats_WPF.Utilities
{
    /// <summary>
    /// Enum values to aide in automated decision-making.
    /// </summary>
    public enum YValueDecisions
    {
        DoNothing,
        DropZeros,
        ChangeHundredth,
        OnePercentLowest
    }

    /// <summary>
    /// Enum values to aide in automated decision-making.
    /// </summary>
    public enum XValueDecisions
    {
        DoNothing,
        ChangeHundredth,
        DropZeros
    }

    /// <summary>
    /// Enum values to aide in automated decision-making.
    /// </summary>
    public enum KValueDecisions
    {
        DeriveValues,
        UseSuppliedValues
    }
}
