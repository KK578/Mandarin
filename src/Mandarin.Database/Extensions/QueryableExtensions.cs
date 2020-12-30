using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mandarin.Database.Extensions
{
    /// <summary>
    /// Extensions for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Asynchronously creates a <see cref="IReadOnlyList{T}" /> from an <see cref="IQueryable{T}" /> by enumerating
        /// it asynchronously.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="IQueryable{T}" /> to create a list from.</param>
        /// <returns>The task containing a <see cref="IReadOnlyList{T}" /> that contains elements from the input sequence.</returns>
        public static async Task<IReadOnlyList<TSource>> ToReadOnlyListAsync<TSource>(this IQueryable<TSource> source)
        {
            var list = await source.ToListAsync();
            return list.AsReadOnly();
        }
    }
}
