using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mandarin.Client.ViewModels.Extensions
{
    /// <summary>
    /// Contains helper methods to apply updates to an <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Clears the given <see cref="ObservableCollection{T}"/> of all current items, and then adds all items in <paramref name="newItems"/> to
        /// <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The observable collection to be reset.</param>
        /// <param name="newItems">The new items to be added to <paramref name="source"/>.</param>
        /// <typeparam name="T">The type of the item.</typeparam>
        public static void Reset<T>(this ObservableCollection<T> source, IEnumerable<T> newItems)
        {
            source.Clear();
            foreach (var item in newItems)
            {
                source.Add(item);
            }
        }
    }
}
