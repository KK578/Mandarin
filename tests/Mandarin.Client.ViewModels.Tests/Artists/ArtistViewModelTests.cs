﻿using FluentAssertions;
using Mandarin.Client.ViewModels.Artists;
using Mandarin.Tests.Data;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Artists
{
    public class ArtistViewModelTests
    {
        [Fact]
        public void ShouldRoundTripAnExistingStockist()
        {
            var subject = new ArtistViewModel(WellKnownTestData.Stockists.OthilieMapples);
            subject.ToStockist().Should().BeEquivalentTo(WellKnownTestData.Stockists.OthilieMapples,
                                                         o => o.Excluding(x => x.Commission.InsertedAt));
        }
    }
}
