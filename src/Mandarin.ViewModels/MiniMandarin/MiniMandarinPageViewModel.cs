using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.MiniMandarin
{
    internal sealed class MiniMandarinPageViewModel : IMiniMandarinPageViewModel
    {
        public MiniMandarinPageViewModel()
        {
            this.Paragraphs = new List<string>
            {
                "The Mini Mandarin is the younger sister of The Little Mandarin and is run by the baker of yummy treats in the family. Come drop by for handmade and hand finished ‘bearcarons’ – bear shaped macarons! – choose from three delightful flavours, chocolate, strawberry and matcha! The Mini Mandarin also has a range of sweet snacks and drinks from Asia to enjoy!"
            }.AsReadOnly();

            this.BannerImageViewModel = new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Banner.jpg", "The Mini Mandarin - Bearcarons");
            this.MacaronImageViewModels = new List<IMandarinImageViewModel>
            {
                new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Macaron-Chocolate.jpg", "The Mini Mandarin - Chocolate Macaron"),
                new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Macaron-Strawberry.jpg", "The Mini Mandarin - Strawberry Macaron"),
                new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Macaron-Matcha.jpg", "The Mini Mandarin - Matcha Macaron"),
            }.AsReadOnly();
        }

        public IReadOnlyList<string> Paragraphs { get; }
        public IMandarinImageViewModel BannerImageViewModel { get; }
        public IReadOnlyList<IMandarinImageViewModel> MacaronImageViewModels { get; }
    }
}
