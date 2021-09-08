using Mandarin.Tests.Helpers;
using Mandarin.Tests.Helpers.Square;
using Xunit;

namespace Mandarin.Tests
{
    [CollectionDefinition(nameof(MandarinTestsCollectionFixture))]
    public class MandarinTestsCollectionFixture : ICollectionFixture<MandarinTestFixture>, ICollectionFixture<SquareWireMockFixture>
    {
    }
}
