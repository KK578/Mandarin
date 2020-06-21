using System;

namespace Mandarin.ViewModels.Index.MandarinMap
{
    /// <inheritdoc />
    internal sealed class MandarinMapViewModel : IMandarinMapViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinMapViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public MandarinMapViewModel(PageContentModel pageContentModel)
        {
            this.MapUri = new Uri(pageContentModel.Get<string>("About", "Map", "Url"));
            this.Width = pageContentModel.Get<int>("About", "Map", "Width");
            this.Height = pageContentModel.Get<int>("About", "Map", "Height");
        }

        /// <inheritdoc/>
        public Uri MapUri { get; }

        /// <inheritdoc/>
        public int Width { get; }

        /// <inheritdoc/>
        public int Height { get; }
    }
}
