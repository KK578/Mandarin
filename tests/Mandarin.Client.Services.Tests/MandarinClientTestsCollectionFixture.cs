using Mandarin.Tests.Helpers;
using Xunit;

namespace Mandarin.Client.Services.Tests
{
    [CollectionDefinition(nameof(MandarinClientTestsCollectionFixture))]
    public class MandarinClientTestsCollectionFixture : ICollectionFixture<MandarinTestFixture>
    {
    }
}
