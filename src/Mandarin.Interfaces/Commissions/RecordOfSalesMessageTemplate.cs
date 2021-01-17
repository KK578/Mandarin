namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents a template for a custom message in a <see cref="RecordOfSales"/>.
    /// </summary>
    public record RecordOfSalesMessageTemplate
    {
        /// <summary>
        /// Gets the name of the template.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets the string format of the template to be substituted with names.
        /// </summary>
        public string TemplateFormat { get; init; }
    }
}
