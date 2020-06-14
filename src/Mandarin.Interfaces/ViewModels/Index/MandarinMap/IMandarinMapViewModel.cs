using System;

namespace Mandarin.ViewModels.Index.MandarinMap
{
    /// <summary>
    /// Represents the component content for The Little Mandarin's map.
    /// </summary>
    public interface IMandarinMapViewModel
    {
        /// <summary>
        /// Gets the full uri for the map to be embedded.
        /// </summary>
        Uri MapUri { get; }

        /// <summary>
        /// Gets the width in pixels for the embedded map.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height in pixels for the embedded map.
        /// </summary>
        int Height { get; }
    }
}
