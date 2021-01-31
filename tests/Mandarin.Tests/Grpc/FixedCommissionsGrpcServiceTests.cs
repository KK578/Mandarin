using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Grpc
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class FixedCommissionsGrpcServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public FixedCommissionsGrpcServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IInventoryService Subject => this.Resolve<IInventoryService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveAllFixedCommissions()
        {
            var fixedCommissionAmounts = await this.Subject.GetFixedCommissionAmounts().ToList();
            fixedCommissionAmounts.Should().HaveCount(2);
        }
    }
}
