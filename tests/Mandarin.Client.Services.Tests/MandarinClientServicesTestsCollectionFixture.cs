using Mandarin.Tests.Helpers;
using Xunit;

namespace Mandarin.Client.Services.Tests
{
    [CollectionDefinition(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinClientServicesTestsCollectionFixture : ICollectionFixture<MandarinTestFixture>
    {
    }
}
