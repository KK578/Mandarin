using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Client.ViewModels.Tests.Helpers;
using Mandarin.Inventory;
using Mandarin.Tests.Data;
using Microsoft.AspNetCore.Components;
using Moq;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FramePrices
{
    public class FramePricesEditViewModelTests
    {
        private static readonly Instant Today = Instant.FromUtc(2021, 08, 01, 00, 00, 00);

        private readonly Fixture fixture = new();
        private readonly Mock<IFramePricesService> framePricesService = new();
        private readonly Mock<IProductRepository> productRepository = new();
        private readonly NavigationManager navigationManager = new MockNavigationManager();
        private readonly Mock<IValidator<IFramePriceViewModel>> validator = new();
        private readonly IClock clock = new FakeClock(FramePricesEditViewModelTests.Today);

        private readonly IFramePricesEditViewModel subject;

        protected FramePricesEditViewModelTests()
        {
            this.subject = new FramePricesEditViewModel(this.framePricesService.Object,
                                                        this.productRepository.Object,
                                                        this.navigationManager,
                                                        this.validator.Object,
                                                        this.clock);
        }

        private void GivenValidationResult(ValidationResult validationResult)
        {
            this.validator
                .Setup(x => x.ValidateAsync(It.IsAny<IFramePriceViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
        }

        private void GivenServicesReturnProduct(FramePrice framePrice, Product product)
        {
            this.productRepository.Setup(x => x.GetProductAsync(product.ProductCode)).ReturnsAsync(product);
            this.framePricesService.Setup(x => x.GetFramePriceAsync(product.ProductCode, It.IsAny<Instant>())).ReturnsAsync(framePrice);
        }


        public class IsLoadingTests : FramePricesEditViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<Product>();
                this.productRepository.Setup(x => x.GetProductAsync(It.IsAny<ProductCode>())).Returns(tcs.Task);

                var unused = this.subject.LoadData.Execute(ProductCode.Of("TLM-001")).ToTask();

                this.subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }
        }

        public class SaveTests : FramePricesEditViewModelTests, IAsyncLifetime
        {
            private readonly ValidationResult failure;
            private readonly ValidationResult success;

            public SaveTests()
            {
                this.failure = new ValidationResult(this.fixture.CreateMany<ValidationFailure>());
                this.success = new ValidationResult();
            }

            /// <inheritdoc />
            public async Task InitializeAsync()
            {
                this.GivenServicesReturnProduct(WellKnownTestData.FramePrices.Clementine,
                                                WellKnownTestData.Products.ClementineFramed);
                await this.subject.LoadData.Execute(WellKnownTestData.Products.ClementineFramed.ProductCode);
            }

            /// <inheritdoc />
            public Task DisposeAsync()
            {
                return Task.CompletedTask;
            }

            [Fact]
            public async Task ShouldBeExecutableOnCreation()
            {
                var canExecute = await this.subject.Save.CanExecute.FirstAsync();
                canExecute.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldUpdateValidationResultOnSaving()
            {
                this.GivenValidationResult(this.failure);

                await this.subject.Save.Execute();
                this.subject.ValidationResult.Should().Be(this.failure);
            }

            [Fact]
            public async Task ShouldNotCallServiceIfValidationFails()
            {
                this.GivenValidationResult(this.failure);
                await this.subject.Save.Execute();
                this.framePricesService.Verify(x => x.SaveFramePriceAsync(It.IsAny<FramePrice>()), Times.Never());
            }

            [Fact]
            public async Task ShouldCallServiceOnSuccessfulValidation()
            {
                this.GivenValidationResult(this.success);
                await this.subject.Save.Execute();
                this.framePricesService.Verify(x => x.SaveFramePriceAsync(It.IsAny<FramePrice>()), Times.Once());
            }

            [Fact]
            public async Task ShouldNavigateToEditNewArtistOnSuccess()
            {
                this.GivenValidationResult(this.success);
                await this.subject.Save.Execute();
                this.navigationManager.Uri.Should().EndWith("/inventory/frame-prices");
            }
        }

        public class CancelTests : FramePricesEditViewModelTests
        {
            [Fact]
            public async Task ShouldBeExecutableOnConstruction()
            {
                var canExecute = await this.subject.Cancel.CanExecute.FirstAsync();
                canExecute.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldNavigateToIndexOnCancel()
            {
                await this.subject.Cancel.Execute();
                this.navigationManager.Uri.Should().EndWith("/inventory/frame-prices");
            }
        }
    }
}
