using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FramePricesNewViewModelTests
    {
        private static readonly Instant Today = Instant.FromUtc(2021, 08, 03, 12, 30, 00);

        private readonly Fixture fixture = new();
        private readonly Mock<IFramePricesService> framePricesService = new();
        private readonly Mock<IProductRepository> productRepository = new();
        private readonly Mock<IValidator<IFramePriceViewModel>> validator = new();
        private readonly NavigationManager navigationManager = new MockNavigationManager();
        private readonly IClock clock = new FakeClock(FramePricesNewViewModelTests.Today);

        private readonly IFramePricesNewViewModel subject;

        protected FramePricesNewViewModelTests()
        {
            this.subject = new FramePricesNewViewModel(this.framePricesService.Object,
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

        public class IsLoadingTests : FramePricesNewViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<IReadOnlyList<Product>>();
                this.productRepository.Setup(x => x.GetAllProductsAsync()).Returns(tcs.Task);

                var unused = this.subject.LoadData.Execute().ToTask();

                this.subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldHaveAvailableListOfProductsAfterLoad()
            {
                var products = this.fixture.CreateMany<Product>().ToList().AsReadOnly();
                this.productRepository.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);

                await this.subject.LoadData.Execute();

                this.subject.Products.Should().BeEquivalentTo(products);
            }
        }

        public class SelectedProductTests : FramePricesNewViewModelTests
        {
            [Fact]
            public void ShouldUpdateFramePriceDetailsOnChangingProduct()
            {
                this.subject.SelectedProduct = WellKnownTestData.Products.Mandarin;

                this.subject.FramePrice.ProductCode.Should().Be("TLM-001");
                this.subject.FramePrice.ProductName.Should().Be("[TLM-001] Mandarin");
                this.subject.FramePrice.RetailPrice.Should().Be(45.00M);
            }
        }

        public class SaveTests : FramePricesNewViewModelTests
        {
            private readonly ValidationResult failure;
            private readonly ValidationResult success;

            public SaveTests()
            {
                this.failure = new ValidationResult(this.fixture.CreateMany<ValidationFailure>());
                this.success = new ValidationResult();

                this.subject.SelectedProduct = WellKnownTestData.Products.Mandarin;
                this.subject.FramePrice.FramePrice = 20.00M;
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
            public async Task ShouldNavigateToEditPageOnSuccess()
            {
                this.GivenValidationResult(this.success);
                await this.subject.Save.Execute();
                this.navigationManager.Uri.Should().EndWith($"/inventory/frame-prices/edit/TLM-001");
            }
        }

        public class CancelTests : FramePricesNewViewModelTests
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
