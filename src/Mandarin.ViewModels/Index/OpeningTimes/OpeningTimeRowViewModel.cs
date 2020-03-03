using System;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    internal sealed class OpeningTimeRowViewModel : IOpeningTimeRowViewModel
    {
        public OpeningTimeRowViewModel(string nameOfDay, DateTime openTime, DateTime closingTime)
            : this(nameOfDay, $"{openTime:h:mmtt} - {closingTime:h:mmtt}")
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
