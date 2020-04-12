using System;
using System.Globalization;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    internal sealed class OpeningTimeRowViewModel : IOpeningTimeRowViewModel
    {
        public OpeningTimeRowViewModel(string nameOfDay, DateTime openTime, DateTime closingTime)
            : this(nameOfDay, $"{OpeningTimeRowViewModel.FormatDateTime(openTime)} - {OpeningTimeRowViewModel.FormatDateTime(closingTime)}")
        {
        }

        public OpeningTimeRowViewModel(string nameOfDay, string message)
        {
            this.NameOfDay = nameOfDay;
            this.Message = message;
        }

        public string NameOfDay { get; }
        public string Message { get; }

        private static string FormatDateTime(DateTime dateTime) => dateTime.ToString("h:mmtt", CultureInfo.CurrentUICulture).ToLower();
    }
}
