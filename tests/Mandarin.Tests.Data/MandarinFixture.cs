using AutoFixture;
using LazyEntityGraph.AutoFixture;
using LazyEntityGraph.EntityFrameworkCore;
using Mandarin.Services.Entity;

namespace Mandarin.Tests.Data
{
    public class MandarinFixture : Fixture
    {
        /// <inheritdoc cref="AutoFixture.Fixture"/>
        private MandarinFixture()
        {
            var modelMetadata = ModelMetadataGenerator.LoadFromContext<MandarinDbContext>(o => new MandarinDbContext(o));
            this.Behaviors.Add(new OmitOnRecursionBehavior());
            this.Customize(new LazyEntityGraphCustomization(modelMetadata));
        }

        public static MandarinFixture Instance { get; } = new MandarinFixture();
    }
}
