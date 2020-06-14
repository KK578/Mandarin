using System;
using System.Collections.Generic;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    /// <inheritdoc />
    internal sealed class OpeningTimesViewModel : IOpeningTimesViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningTimesViewModel"/> class.
        /// </summary>
        public OpeningTimesViewModel()
        {
            this.OpeningTimes = new List<OpeningTimeRowViewModel>
            {
                new OpeningTimeRowViewModel("Monday", "Closed"),
                new OpeningTimeRowViewModel("Tuesday", "Closed"),
                new OpeningTimeRowViewModel("Wednesday", "Closed"),
                new OpeningTimeRowViewModel("Thursday", "Temporarily Closed"),
                new OpeningTimeRowViewModel("Friday", "Temporarily Closed"),
                new OpeningTimeRowViewModel("Saturday", "Temporarily Closed"),
                new OpeningTimeRowViewModel("Sunday", "Temporarily Closed"),
            };
        }

        /// <inheritdoc/>
        public IReadOnlyList<IOpeningTimeRowViewModel> OpeningTimes { get; }
    }
}
