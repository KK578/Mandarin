using System.Collections.Generic;

namespace Mandarin.ViewModels
{
    internal sealed class IndexPageViewModel : IIndexPageViewModel
    {
        public IReadOnlyList<string> Paragraphs { get; }

        public IndexPageViewModel()
        {
            Paragraphs = new List<string>
            {
                "The Little Mandarin is a family-run Art and Illustration Shop and Confectionary found in the heart of Walthamstow Village, E17. The owner, Eileen Kai Hing Kwan is a local Illustrator who has lived in Walthamstow for most of her life and after taking over from her parents, the previous owners, this makes The Little Mandarin the third chapter of the family’s shop.",
                "The Little Mandarin believes in welcoming and supporting all types of artists and styles of art, bringing a range of art prints, framed art, notebooks, gifts and more, from our little space in the village into the thriving creative community of Walthamstow.",
                "We are also the proud hosts of, The Mini Mandarin, selling handmade and hand finished ‘bearcarons’ (bear shaped macarons!) alongside a variety of sweet snacks and drinks from Asia.",
                "We hope to see you soon!"
            }.AsReadOnly();
        }
    }
}
