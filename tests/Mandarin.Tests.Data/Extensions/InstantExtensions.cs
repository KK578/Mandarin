using NodaTime;

namespace Mandarin.Tests.Data.Extensions
{
    public static class InstantExtensions
    {
        public static Instant PlusMilliseconds(this Instant instant, int milliseconds)
        {
            return instant.PlusNanoseconds(milliseconds * 1_000_000);
        }
    }
}
