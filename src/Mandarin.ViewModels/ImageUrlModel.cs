namespace Mandarin.ViewModels
{
    /// <summary>
    /// Represents a backing model for an image url.
    /// </summary>
    internal class ImageUrlModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageUrlModel"/> class.
        /// </summary>
        /// <param name="url">The image's url.</param>
        /// <param name="description">The image's description.</param>
        public ImageUrlModel(string url, string description)
        {
            this.Url = url;
            this.Description = description;
        }

        /// <summary>
        /// Gets the image's url.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Gets the image's description.
        /// </summary>
        public string Description { get; }
    }
}
