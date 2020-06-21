using System;

namespace Mandarin.ViewModels
{
    /// <summary>
    /// Represents a backing model for an opening time for a specific day.
    /// </summary>
    internal sealed class OpeningTimeModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningTimeModel"/> class.
        /// </summary>
        /// <param name="name">The name of the day this opening time is for.</param>
        /// <param name="message">The optional message of the opening time, which overrides opening/closing times.</param>
        /// <param name="open">The time of day for opening.</param>
        /// <param name="close">The time of day for closing. </param>
        public OpeningTimeModel(string name, string message, DateTime? open, DateTime? close)
        {
            this.Name = name;
            this.Message = message;
            this.Open = open;
            this.Close = close;
        }

        /// <summary>
        /// Gets the name of the day.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a custom overriding message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the time of opening.
        /// </summary>
        public DateTime? Open { get; }

        /// <summary>
        /// Gets the time of closing.
        /// </summary>
        public DateTime? Close { get; }
    }
}
