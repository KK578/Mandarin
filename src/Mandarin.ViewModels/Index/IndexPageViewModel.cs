using Mandarin.ViewModels.Components.Images;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.Index
{
    /// <inheritdoc />
    internal sealed class IndexPageViewModel : IIndexPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPageViewModel"/> class.
        /// </summary>
        /// <param name="markdownPipeline">The markdown pipeline.</param>
        public IndexPageViewModel(MarkdownPipeline markdownPipeline)
        {
            var mainContent = IndexPageViewModel.CreateMainContent();
            var giftCardContent = IndexPageViewModel.CreateGiftCardContent();
            this.MainContent = new MarkupString(Markdown.ToHtml(mainContent, markdownPipeline));
            this.GiftCardContent = new MarkupString(Markdown.ToHtml(giftCardContent, markdownPipeline));
            this.GiftCardImageViewModel = new MandarinImageViewModel("/static/images/about/GiftCards.gif", "The Little Mandarin - Gift Card Designs");
        }

        /// <inheritdoc/>
        public MarkupString MainContent { get; }

        /// <inheritdoc/>
        public MarkupString GiftCardContent { get; }

        /// <inheritdoc/>
        public IMandarinImageViewModel GiftCardImageViewModel { get; }

        private static string CreateMainContent()
        {
            return @"----

The Little Mandarin is an independent Art and Sweets Shop based in East London, found in the heart of Walthamstow 
Village, E17. The owner, Eileen Kai Hing Kwan is a local Illustrator who has lived in Walthamstow for most of her 
life and after taking over from her parents, the previous owners, this makes The Little Mandarin the third chapter 
of the family’s shop.

The Little Mandarin believes in welcoming and supporting all types of artists and styles of art, bringing a range of 
art prints, framed art, greetings cards, gifts and more, from our little space in the village into the thriving 
creative community of Walthamstow and further.

We are also the proud hosts of, The Mini Mandarin, selling handmade and hand finished 'bearcarons' (bear shaped 
macarons!) alongside a variety of sweets and chocolates suitable for vegetarians and vegans!

We hope to see you soon!

----";
        }

        private static string CreateGiftCardContent()
        {
            return @"### eGift Cards

The Little Mandarin is temporarily closed due to COVID-19, and we unfortunately do not have the means to move online. 
The kindness and support we've received has been overwhelming and humbling, and we are happy to say that you can now 
support our independent and family-run shop by purchasing one of our eGift Cards online!

There are several cute designs to choose from and the eGift Card is emailed directly to you or your giftee and can be 
redeemed in-store after we reopen. Our eGift Cards are non refundable and can only be redeemed in store but they do not 
expire, so there’s no rush to use them! Your eGift Card(s) can be redeemed in-store to make full or partial payments, 
and checking any remaining balance can be easily done and at any time via our website.

We want to sincerely thank everyone for their amazing support and kindness during this time and we look forward to 
seeing you all again for art and sweets when it is safe for us to reopen.

Until then, please take care and stay safe!";
        }
    }
}
