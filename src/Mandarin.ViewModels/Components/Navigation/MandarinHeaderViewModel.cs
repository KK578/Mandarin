using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Components.Navigation
{
    internal sealed class MandarinHeaderViewModel : IMandarinHeaderViewModel
    {
        public MandarinHeaderViewModel()
        {
            this.LogoImageViewModel = new MandarinImageViewModel("/static/images/logo.png", "The Little Mandarin");
        }

        public IMandarinImageViewModel LogoImageViewModel { get; }
    }
}
