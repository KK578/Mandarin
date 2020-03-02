using System.Collections.Generic;

namespace Mandarin.ViewModels
{
    public interface IOpeningTimesViewModel
    {
        IReadOnlyList<OpeningTime> OpeningTimes { get; }
    }
}