using System;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    public class OpeningTimeRowViewModel : IOpeningTimeRowViewModel
    {
        public OpeningTimeRowViewModel(string nameOfDay, DateTime openTime, DateTime closingTime)
            : this(nameOfDay, $"{openTime:HH:mm} - {closingTime:HH:mm}")
        {
        }

        public OpeningTimeRowViewModel(string nameOfDay, string message)
        {
            NameOfDay = nameOfDay;
            Message = message;
        }

        public string NameOfDay { get; }
        public string Message { get; }
    }
}
