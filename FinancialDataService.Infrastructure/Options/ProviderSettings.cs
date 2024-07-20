namespace FinancialDataService.Infrastructure.Options
{
    /// <summary>
    /// Represents the settings for a provider.
    /// </summary>
    public class ProviderSettings
    {
        /// <summary>
        /// Gets or sets the list of supported symbols.
        /// </summary>
        /// <value>
        /// The list of supported symbols.
        /// </value>
        public List<string> SupportedSymbols { get; set; }

        /// <summary>
        /// Gets or sets the cron expression for fetching instruments used by the job.
        /// </summary>
        /// <value>
        /// The cron expression for fetching instruments.
        /// </value>
        public string FetchInstrumentsJobCronExpression { get; set; }
    }
}