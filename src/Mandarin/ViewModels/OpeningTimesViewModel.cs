using System;
using System.Collections.Generic;

namespace Mandarin.ViewModels
{
    public interface IOpeningTimesViewModel
    {
        IReadOnlyList<OpeningTime> OpeningTimes { get; }
    }

    internal sealed class OpeningTimesViewModel : IOpeningTimesViewModel
    {
        public IReadOnlyList<OpeningTime> OpeningTimes { get; }

        public OpeningTimesViewModel()
        {
            var time1230 = new DateTime(2020, 06, 01, 12, 30, 0, DateTimeKind.Utc);
            var time1300 = new DateTime(2020, 06, 01, 13, 00, 0, DateTimeKind.Utc);
            var time1600 = new DateTime(2020, 06, 01, 16, 00, 0, DateTimeKind.Utc);
            var time1730 = new DateTime(2020, 06, 01, 17, 30, 0, DateTimeKind.Utc);

            OpeningTimes = new List<OpeningTime>
            {
                new OpeningTime("Monday", "Closed"),
                new OpeningTime("Tuesday", "Closed"),
                new OpeningTime("Wednesday", "Closed"),
                new OpeningTime("Thursday", time1230, time1730),
                new OpeningTime("Friday", time1230, time1730),
                new OpeningTime("Saturday", time1230, time1730),
                new OpeningTime("Sunday", time1300, time1600),

            };
        }
    }

    public class OpeningTime
    {
        public OpeningTime(string nameOfDay, DateTime openTime, DateTime closingTime)
            : this(nameOfDay, $"{openTime:HH:mm} - {closingTime:HH:mm}")
        {
        }

        public OpeningTime(string nameOfDay, string message)
        {
            NameOfDay = nameOfDay;
            Message = message;
        }

        public string NameOfDay { get; }
        public string Message { get; }
    }
}
