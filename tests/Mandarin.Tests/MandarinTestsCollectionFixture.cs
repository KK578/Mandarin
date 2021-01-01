using Mandarin.Tests.Helpers;
using Xunit;

namespace Mandarin.Tests
{
    [CollectionDefinition(nameof(MandarinTestsCollectionFixture))]
    public class MandarinTestsCollectionFixture : ICollectionFixture<MandarinTestFixture>
    {
    }
}
