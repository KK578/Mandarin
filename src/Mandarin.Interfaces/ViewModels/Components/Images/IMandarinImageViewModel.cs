using System;

namespace Mandarin.ViewModels.Components.Images
{
    /// <summary>
    /// Represents the component content for an image.
    /// </summary>
    public interface IMandarinImageViewModel
    {
        /// <summary>
        /// Gets the uri to the image.
        /// </summary>
        Uri SourceUrl { get; }

        /// <summary>
        /// Gets the image's alt description.
        /// </summary>
        string Description { get; }
    }
}
