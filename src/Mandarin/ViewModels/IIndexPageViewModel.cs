using System.Collections.Generic;

namespace Mandarin.ViewModels
{
    public interface IIndexPageViewModel
    {
        IReadOnlyList<string> Paragraphs { get; }
    }
}
