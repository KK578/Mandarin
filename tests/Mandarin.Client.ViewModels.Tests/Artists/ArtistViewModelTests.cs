using FluentAssertions;
using Mandarin.Client.ViewModels.Artists;
using Mandarin.Tests.Data;
using Mandarin.Tests.Data.Extensions;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Artists
{
    public class ArtistViewModelTests
    {
        [Fact]
        public void ShouldRoundTripAnExistingStockist()
        {
            var subject = new ArtistViewModel(WellKnownTestData.Stockists.OthilieMapples);
            subject.ToStockist().Should().MatchStockist(WellKnownTestData.Stockists.OthilieMapples);
        }
    }
}
