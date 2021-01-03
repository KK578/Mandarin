using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Services;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Grpc
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class CommissionsGrpcServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public CommissionsGrpcServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private ICommissionService Subject => this.Resolve<ICommissionService>();

        [Fact]
        public async Task ShouldReceiveCommissionRateGroupsInAscendingOrder()
        {
            var rateGroups = await this.Subject.GetCommissionRateGroupsAsync();
            rateGroups.Select(x => x.Rate).Should().BeInAscendingOrder();
        }
    }
}
