using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Mandarin.Client.ViewModels.Artists;
using Mandarin.Client.ViewModels.Tests.Helpers;
using Mandarin.Common;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Artists
{
    public class ArtistsEditViewModelTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IStockistService> stockistService = new();
        private readonly MockNavigationManager navigationManager = new();
        private readonly Mock<IValidator<IArtistViewModel>> artistValidator = new();

        private readonly IArtistsEditViewModel subject;

        protected ArtistsEditViewModelTests()
        {
            this.subject = new ArtistsEditViewModel(this.stockistService.Object, this.navigationManager, this.artistValidator.Object);
        }

        private void GivenServiceReturnsStockist(Stockist stockist)
        {
            this.stockistService.Setup(x => x.GetStockistByCodeAsync(stockist.StockistCode)).ReturnsAsync(stockist);
        }

        private void GivenValidationResult(ValidationResult validationResult)
        {
            this.artistValidator
                .Setup(x => x.ValidateAsync(It.IsAny<IArtistViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
        }

        public class IsLoadingTests : ArtistsEditViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<Stockist>();
                this.stockistService.Setup(x => x.GetStockistByCodeAsync(It.IsAny<string>())).Returns(tcs.Task);

                var unused = this.subject.LoadData.Execute().ToTask();

                this.subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldBeFalseAfterLoadingFinishes()
            {
                this.GivenServiceReturnsStockist(WellKnownTestData.Stockists.KelbyTynan);

                await this.subject.LoadData.Execute(WellKnownTestData.Stockists.KelbyTynan.StockistCode);

                this.subject.IsLoading.Should().BeFalse();
            }
        }

        public class StockistTests : ArtistsEditViewModelTests
        {
            [Fact]
            public async Task ShouldPopulateStockistAfterLoadingData()
            {
                this.GivenServiceReturnsStockist(WellKnownTestData.Stockists.OthilieMapples);

                await this.subject.LoadData.Execute(WellKnownTestData.Stockists.OthilieMapples.StockistCode);

                this.subject.Stockist.StockistId.Should().Be(4);
                this.subject.Stockist.StockistCode.Should().Be("OM19");
                this.subject.Stockist.StatusCode.Should().Be(StatusMode.ActiveHidden);
                this.subject.Stockist.FirstName.Should().Be("Othilie");
                this.subject.Stockist.LastName.Should().Be("Mapples");
                this.subject.Stockist.DisplayName.Should().Be("Othilie Mapples");
                this.subject.Stockist.TwitterHandle.Should().Be("ropfer3");
                this.subject.Stockist.InstagramHandle.Should().Be("ropfer3");
                this.subject.Stockist.FacebookHandle.Should().BeNull();
                this.subject.Stockist.WebsiteUrl.Should().Be("http://mtv.com/non/mauris/morbi.jsp");
                this.subject.Stockist.TumblrHandle.Should().BeNull();
                this.subject.Stockist.EmailAddress.Should().Be("jgunny3@unicef.org");
                this.subject.Stockist.CommissionId.Should().Be(4);
                this.subject.Stockist.StartDate.Should().Be(new DateTime(2019, 01, 16));
                this.subject.Stockist.EndDate.Should().Be(new DateTime(2019, 04, 16));
                this.subject.Stockist.Rate.Should().Be(40);
            }
        }

        public class StatusesTests : ArtistsEditViewModelTests
        {
            [Fact]
            public void ShouldHaveCorrectAvailableOptions()
            {
                var statuses = this.subject.Statuses;
                statuses.Should().BeEquivalentTo(StatusMode.Inactive, StatusMode.ActiveHidden, StatusMode.Active);
            }
        }

        public class SaveTests : ArtistsEditViewModelTests, IAsyncLifetime
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
                this.GivenServiceReturnsStockist(WellKnownTestData.Stockists.ArlueneWoodes);
                await this.subject.LoadData.Execute(WellKnownTestData.Stockists.ArlueneWoodes.StockistCode);
            }

            /// <inheritdoc />
            public Task DisposeAsync()
            {
                return Task.CompletedTask;
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
                this.stockistService.Verify(x => x.SaveStockistAsync(It.IsAny<Stockist>()), Times.Never());
            }

            [Fact]
            public async Task ShouldCallServiceOnSuccessfulValidation()
            {
                this.GivenValidationResult(this.success);
                await this.subject.Save.Execute();
                this.stockistService.Verify(x => x.SaveStockistAsync(It.IsAny<Stockist>()), Times.Once());
            }

            [Fact]
            public async Task ShouldNavigateToEditEditArtistOnSuccess()
            {
                this.GivenValidationResult(this.success);

                this.subject.Stockist.StockistCode = "TLM";
                await this.subject.Save.Execute();

                this.navigationManager.Uri.Should().EndWith("/artists");
            }
        }

        public class CancelTests : ArtistsEditViewModelTests
        {
            [Fact]
            public async Task ShouldRedirectToIndexPageOnCancelling()
            {
                await this.subject.Cancel.Execute();
                this.navigationManager.Uri.Should().EndWith("/artists");
            }
        }
    }
}
