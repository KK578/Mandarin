using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Grpc.Core;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Mandarin.Api.Inventory;
using Mandarin.Grpc;
using Mandarin.Inventory;
using Moq;
using Xunit;

namespace Mandarin.Tests.Grpc
{
    public class ProductsGrpcServiceTests
    {
        private readonly Mock<IProductRepository> productRepository = new();
        private readonly Mock<IMapper> mapper = new();
        private readonly Mock<IBackgroundJobClient> backgroundJobClient = new();
        private readonly ProductsGrpcService subject;

        protected ProductsGrpcServiceTests()
        {
            this.subject = new ProductsGrpcService(this.productRepository.Object, this.mapper.Object, this.backgroundJobClient.Object);
        }

        private Task<Job> GivenHangfireCapturesJob()
        {
            var tcs = new TaskCompletionSource<Job>();
            this.backgroundJobClient.Setup(x => x.Create(It.IsAny<Job>(), It.IsAny<IState>()))
                .Callback((Job job, IState _) => tcs.SetResult(job));

            return tcs.Task;
        }

        public class SynchronizeProductsTests : ProductsGrpcServiceTests
        {
            [Fact]
            public async Task ShouldEnqueueCorrectJobOnSynchronizeProduct()
            {
                var jobTask = this.GivenHangfireCapturesJob();
                await this.subject.SynchronizeProducts(new SynchronizeProductsRequest(), Mock.Of<ServerCallContext>());
                var job = await jobTask;
                job.Type.Should().Be<IProductSynchronizer>();
                job.Method.Name.Should().Be(nameof(IProductSynchronizer.SynchronizeProductsAsync));
            }
        }
    }
}
