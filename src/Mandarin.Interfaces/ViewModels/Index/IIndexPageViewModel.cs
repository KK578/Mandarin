using System.Collections.Generic;

namespace Mandarin.ViewModels.Index
{
    public interface IIndexPageViewModel
    {
        IReadOnlyList<string> Paragraphs { get; }
    }
}
