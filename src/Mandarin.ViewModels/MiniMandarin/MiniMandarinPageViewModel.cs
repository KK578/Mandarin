using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.MiniMandarin
{
    /// <inheritdoc />
    internal sealed class MiniMandarinPageViewModel : IMiniMandarinPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MiniMandarinPageViewModel"/> class.
        /// </summary>
        /// <param name="markdownPipeline">The markdown pipeline.</param>
        public MiniMandarinPageViewModel(MarkdownPipeline markdownPipeline)
        {
            var textContent = MiniMandarinPageViewModel.CreateTextContent();
            this.TextContent = new MarkupString(Markdown.ToHtml(textContent, markdownPipeline));
            this.BannerImageViewModel = new MandarinImageViewModel("/static/images/the-mini-mandarin/TheMiniMandarin-Banner.jpg", "The Mini Mandarin - Bearcarons");
            this.MacaronImageViewModels = new List<IMandarinImageViewModel>
            {
                new MandarinImageViewModel("/static/images/the-mini-mandarin/TheMiniMandarin-Macaron-Chocolate.jpg", "The Mini Mandarin - Chocolate Macaron"),
                new MandarinImageViewModel("/static/images/the-mini-mandarin/TheMiniMandarin-Macaron-Strawberry.jpg", "The Mini Mandarin - Strawberry Macaron"),
                new MandarinImageViewModel("/static/images/the-mini-mandarin/TheMiniMandarin-Macaron-Matcha.jpg", "The Mini Mandarin - Matcha Macaron"),
            }.AsReadOnly();
        }

        /// <inheritdoc/>
        public MarkupString TextContent { get; }

        /// <inheritdoc/>
        public IMandarinImageViewModel BannerImageViewModel { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IMandarinImageViewModel> MacaronImageViewModels { get; }

        private static string CreateTextContent()
        {
            return @"The Mini Mandarin is the younger sister of The Little Mandarin and is run by the baker of yummy
treats in the family. Come drop by for handmade and hand finished ‘bearcarons’ – bear shaped macarons! Choose from
three delightful flavours, chocolate, strawberry and matcha! Other special flavours pop up all year round and JUMBO
sized macarons too, so come back to see what's new! The Mini Mandarin also offers a range of sweets and chocolates 
suitable for vegetarians and vegans - there's a treat for everyone!

You can follow The Mini Mandarin on Instagram [@theminimandarin_e17](https://instagram.com/theminimandarin_e17) to make
sure you don't miss out!";
        }
    }
}
