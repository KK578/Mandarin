using System.Threading.Tasks;
using FluentAssertions;
using Grpc.Core;
using Mandarin.Inventory;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Inventory
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinGrpcFixedCommissionServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public MandarinGrpcFixedCommissionServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IFixedCommissionService Subject => this.Resolve<IFixedCommissionService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveAllFixedCommissions()
        {
            var fixedCommissionAmounts = await this.Subject.GetFixedCommissionAsync();
            fixedCommissionAmounts.Should().HaveCount(2);
        }

        [Fact]
        public async Task ShouldBeAbleToAddAndRoundTripANewFixedCommission()
        {
            var fixedCommissionAmount = new FixedCommissionAmount("OM19-001", 15.00M);
            await this.Subject.SaveFixedCommissionAsync(fixedCommissionAmount);

            var newFixedCommissionAmount = await this.Subject.GetFixedCommissionAsync("OM19-001");
            newFixedCommissionAmount.Should().BeEquivalentTo(fixedCommissionAmount);
        }

        [Fact]
        public async Task ShouldBeAbleToDeleteAFixedCommission()
        {
            await this.Subject.DeleteFixedCommissionAsync("KT20-001F");

            await this.Subject.Invoking(x => x.GetFixedCommissionAsync("KT20-001F")).Should().ThrowAsync<RpcException>();
            await this.Subject.Awaiting(x => x.GetFixedCommissionAsync("KT20-002F")).Should().NotThrowAsync();
        }
    }
}
