namespace Mandarin.ViewModels.Home.OpeningTimes
{
    /// <summary>
    /// Gets the component content for a specific day in the opening times.
    /// </summary>
    public interface IOpeningTimeRowViewModel
    {
        /// <summary>
        /// Gets the name of the day to be displayed. Usually a day of the week.
        /// </summary>
        string NameOfDay { get; }

        /// <summary>
        /// Gets the message to display for this day. Usually "Closed" or opening hours (e.g. 12:00 - 17:00).
        /// </summary>
        string Message { get; }
    }
}
