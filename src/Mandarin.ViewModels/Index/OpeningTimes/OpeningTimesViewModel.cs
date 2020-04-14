using System;
using System.Collections.Generic;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    internal sealed class OpeningTimesViewModel : IOpeningTimesViewModel
    {
        public IReadOnlyList<IOpeningTimeRowViewModel> OpeningTimes { get; }

        public OpeningTimesViewModel()
        {
            // var time1230 = new DateTime(2020, 06, 01, 12, 30, 0, DateTimeKind.Utc);
            // var time1300 = new DateTime(2020, 06, 01, 13, 00, 0, DateTimeKind.Utc);
            // var time1600 = new DateTime(2020, 06, 01, 16, 00, 0, DateTimeKind.Utc);
            // var time1730 = new DateTime(2020, 06, 01, 17, 30, 0, DateTimeKind.Utc);

            this.OpeningTimes = new List<OpeningTimeRowViewModel>
            {
                new OpeningTimeRowViewModel("Monday", "Closed"),
                new OpeningTimeRowViewModel("Tuesday", "Closed"),
                new OpeningTimeRowViewModel("Wednesday", "Closed"),
                new OpeningTimeRowViewModel("Thursday", "Temporarily Closed"),
                new OpeningTimeRowViewModel("Friday", "Temporarily Closed"),
                new OpeningTimeRowViewModel("Saturday", "Temporarily Closed"),
                new OpeningTimeRowViewModel("Sunday", "Temporarily Closed"),
                // new OpeningTimeRowViewModel("Thursday", time1230, time1730),
                // new OpeningTimeRowViewModel("Friday", time1230, time1730),
                // new OpeningTimeRowViewModel("Saturday", time1230, time1730),
                // new OpeningTimeRowViewModel("Sunday", time1300, time1600),
            };
        }
    }
}
