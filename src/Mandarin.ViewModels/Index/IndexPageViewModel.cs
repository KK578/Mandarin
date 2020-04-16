using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Index
{
    internal sealed class IndexPageViewModel : IIndexPageViewModel
    {
        public IReadOnlyList<string> Paragraphs { get; }
        public IMandarinImageViewModel GiftCardImageViewModel { get; }

        public IndexPageViewModel()
        {
            this.Paragraphs = new List<string>
            {
                "The Little Mandarin is an independent Art and Sweets Shop based in East London, found in the heart of Walthamstow Village, E17. The owner, Eileen Kai Hing Kwan is a local Illustrator who has lived in Walthamstow for most of her life and after taking over from her parents, the previous owners, this makes The Little Mandarin the third chapter of the family’s shop.",
                "The Little Mandarin believes in welcoming and supporting all types of artists and styles of art, bringing a range of art prints, framed art, greetings cards, gifts and more, from our little space in the village into the thriving creative community of Walthamstow and further.",
                "We are also the proud hosts of, The Mini Mandarin, selling handmade and hand finished 'bearcarons' (bear shaped macarons!) alongside a variety of sweets and chocolates suitable for vegetarians and vegans!",
                "We hope to see you soon!",
            }.AsReadOnly();
            this.GiftCardImageViewModel = new MandarinImageViewModel("/static/images/about/GiftCards.gif", "The Little Mandarin - Gift Card Designs");
        }
    }
}
