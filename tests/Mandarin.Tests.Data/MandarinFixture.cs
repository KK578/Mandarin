using AutoFixture;

namespace Mandarin.Tests.Data
{
    public class MandarinFixture : Fixture
    {
        /// <inheritdoc cref="AutoFixture.Fixture"/>
        private MandarinFixture()
        {
            this.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public static MandarinFixture Instance { get; } = new();
    }
}
