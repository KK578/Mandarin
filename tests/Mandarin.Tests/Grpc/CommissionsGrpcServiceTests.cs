using System.Threading.Tasks;
using Mandarin.Commissions;
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

        [Fact(Skip = "Test not yet implemented.")]
        public Task ShouldReceiveCommissionRateGroupsInAscendingOrder()
        {
            // TODO: Add RecordOfSales tests.
            var unused = this.Subject;
            return Task.CompletedTask;
        }
    }
}
