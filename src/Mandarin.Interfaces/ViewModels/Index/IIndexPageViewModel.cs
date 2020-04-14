using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.Index
{
    public interface IIndexPageViewModel
    {
        IReadOnlyList<string> Paragraphs { get; }
        IMandarinImageViewModel GiftCardImageViewModel { get; }
    }
}
