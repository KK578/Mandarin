using System.Collections.Generic;
using System.Linq;

namespace Mandarin.Services.Square
{
    /// <summary>
    /// Additional Extensions for <see cref="IEnumerable{T}"/>.
    /// </summary>
    internal static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the <paramref name="source"/> enumerable unchanged or an empty enumerable if <paramref name="source"/> is null.
        /// </summary>
        /// <param name="source">The enumerable to be null-checked.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <returns>The <paramref name="source"/> enumerable but guaranteed to not be null.</returns>
        public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}
