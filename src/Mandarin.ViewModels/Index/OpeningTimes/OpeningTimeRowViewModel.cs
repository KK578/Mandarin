using System;
using System.Globalization;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    /// <inheritdoc />
    internal sealed class OpeningTimeRowViewModel : IOpeningTimeRowViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningTimeRowViewModel"/> class for the specified opening times.
        /// </summary>
        /// <param name="nameOfDay">The name of the day.</param>
        /// <param name="openTime">The opening time for the store for the specified day.</param>
        /// <param name="closingTime">The closing time for the store for the specified day.</param>
        public OpeningTimeRowViewModel(string nameOfDay, DateTime openTime, DateTime closingTime)
            : this(nameOfDay,
                   $"{OpeningTimeRowViewModel.FormatDateTime(openTime)} - {OpeningTimeRowViewModel.FormatDateTime(closingTime)}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningTimeRowViewModel"/> class.
        /// </summary>
        /// <param name="nameOfDay">The name of the day.</param>
        /// <param name="message">The custom message to be displayed.</param>
        public OpeningTimeRowViewModel(string nameOfDay, string message)
        {
            this.NameOfDay = nameOfDay;
            this.Message = message;
        }

        /// <inheritdoc/>
        public string NameOfDay { get; }

        /// <inheritdoc/>
        public string Message { get; }

        private static string FormatDateTime(DateTime dateTime) => dateTime.ToString("h:mmtt", CultureInfo.CurrentUICulture).ToLower();
    }
}
