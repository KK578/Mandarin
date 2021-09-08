using Mandarin.Tests.Helpers;
using Mandarin.Tests.Helpers.SendGrid;
using Xunit;

namespace Mandarin.Client.Services.Tests
{
    [CollectionDefinition(nameof(MandarinClientServicesTestsCollectionFixture))]
    public sealed class MandarinClientServicesTestsCollectionFixture : ICollectionFixture<MandarinTestFixture>, ICollectionFixture<SendGridWireMockFixture>
    {
    }
}
