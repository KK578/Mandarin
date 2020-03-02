using System.Collections.Generic;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    public interface IOpeningTimesViewModel
    {
        IReadOnlyList<IOpeningTimeRowViewModel> OpeningTimes { get; }
    }
}
