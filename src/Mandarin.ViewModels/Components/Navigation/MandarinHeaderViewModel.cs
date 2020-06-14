using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Components.Navigation
{
    /// <inheritdoc />
    internal sealed class MandarinHeaderViewModel : IMandarinHeaderViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinHeaderViewModel"/> class.
        /// </summary>
        public MandarinHeaderViewModel()
        {
            this.LogoImageViewModel = new MandarinImageViewModel("/static/images/logo.png", "The Little Mandarin");
        }

        /// <inheritdoc/>
        public IMandarinImageViewModel LogoImageViewModel { get; }
    }
}
