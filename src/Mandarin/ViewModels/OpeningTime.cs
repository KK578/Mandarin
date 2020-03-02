using System;

namespace Mandarin.ViewModels
{
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