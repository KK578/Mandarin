using System.Collections.Generic;
using System.Linq;

namespace Mandarin.ViewModels.Index.OpeningTimes
{
    /// <inheritdoc />
    internal sealed class OpeningTimesViewModel : IOpeningTimesViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpeningTimesViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public OpeningTimesViewModel(PageContentModel pageContentModel)
        {
            this.OpeningTimes = pageContentModel.GetAll<OpeningTimeModel>("About", "OpeningTimes")
                                                .Select(x => new OpeningTimeRowViewModel(x))
                                                .ToList()
                                                .AsReadOnly();
        }

        /// <inheritdoc/>
        public IReadOnlyList<IOpeningTimeRowViewModel> OpeningTimes { get; }
    }
}
