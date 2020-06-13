using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Components.Navigation
{
    /// <summary>
    /// Represents the component content for the public header bar.
    /// </summary>
    public interface IMandarinHeaderViewModel
    {
        /// <summary>
        /// Gets the image details for the main logo.
        /// </summary>
        IMandarinImageViewModel LogoImageViewModel { get; }
    }
}
