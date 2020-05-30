using System.Collections.Generic;

namespace Mandarin.Services.Square
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? new List<T>();
        }
    }
}
