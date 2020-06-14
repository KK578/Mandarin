using System.Collections.Generic;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    /// <summary>
    /// Gets the component content for The Little Mandarin's opening times.
    /// </summary>
    public interface IOpeningTimesViewModel
    {
        /// <summary>
        /// Gets the list of all days to be displayed in the opening times.
        /// </summary>
        IReadOnlyList<IOpeningTimeRowViewModel> OpeningTimes { get; }
    }
}
