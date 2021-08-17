using NodaTime;

namespace Mandarin.Extensions
{
    /// <summary>
    /// Represents helpful extensions on NodaTime types.
    /// </summary>
    public static class NodaTimeExtensions
    {
        /// <summary>
        /// Returns a <see cref="Interval"/> from the start of both days in UTC of the given <see cref="DateInterval"/>.
        /// </summary>
        /// <param name="dateInterval">The date interval to be converted.</param>
        /// <returns>An <see cref="Interval"/> matching the same date interval.</returns>
        public static Interval ToInterval(this DateInterval dateInterval)
        {
            return new Interval(dateInterval.Start.ToInstant(), dateInterval.End.ToInstant());
        }

        /// <summary>
        /// Returns the given <see cref="LocalDate"/> as an <see cref="Instant"/> at the start of day in UTC.
        /// </summary>
        /// <param name="localDate">The local date to be converted.</param>
        /// <returns>An <see cref="Instant"/> at start of the given date in UTC.</returns>
        public static Instant ToInstant(this LocalDate localDate)
        {
            return localDate.AtStartOfDayInZone(DateTimeZone.Utc).ToInstant();
        }

        /// <summary>
        /// Returns a new <see cref="Instant"/> with the given <paramref name="milliseconds"/> added on.
        /// </summary>
        /// <param name="instant">The instant to be added ot.</param>
        /// <param name="milliseconds">The milliseconds to add to <paramref name="instant"/> to create the return value.</param>
        /// <returns>The result of adding the given number of milliseconds to the instant.</returns>
        public static Instant PlusMilliseconds(this Instant instant, int milliseconds)
        {
            return instant.PlusNanoseconds(milliseconds * 1_000_000);
        }

        /// <summary>
        /// Returns the given <see cref="Instant"/> as a <see cref="LocalDate"/> in UTC.
        /// </summary>
        /// <param name="instant">The instant to be converted.</param>
        /// <returns>A <see cref="LocalDate"/> of the given date in UTC.</returns>
        public static LocalDate ToLocalDate(this Instant instant)
        {
            return instant.InUtc().Date;
        }
    }
}
