using System.Reactive.Linq;
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
using Moq;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Artists
{
    public class ArtistsNewViewModelTests
    {
        private static readonly Instant Today = Instant.FromUtc(2021, 08, 08, 12, 20, 00);
        private static readonly LocalDate StartDate = new(2021, 08, 08);
        private static readonly LocalDate EndDate = new(2021, 11, 06);

        private readonly Fixture fixture = new();
        private readonly Mock<IStockistService> stockistService = new();
        private readonly MockNavigationManager navigationManager = new();
        private readonly Mock<IValidator<IArtistViewModel>> artistValidator = new();
        private readonly IClock clock = new FakeClock(ArtistsNewViewModelTests.Today);

        private IArtistsNewViewModel Subject =>
            new ArtistsNewViewModel(this.stockistService.Object, this.navigationManager, this.artistValidator.Object, this.clock);

        private void GivenValidationResult(ValidationResult validationResult)
        {
            this.artistValidator
                .Setup(x => x.ValidateAsync(It.IsAny<IArtistViewModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
        }

        public class StockistTests : ArtistsNewViewModelTests
        {
            [Fact]
            public void ShouldHaveDefaultStockistOnConstruction()
            {
                var stockist = this.Subject.Stockist;
                stockist.StatusCode.Should().Be(StatusMode.Active);
                stockist.Rate.Should().Be(100);
                stockist.StartDate.Should().Be(ArtistsNewViewModelTests.StartDate);
                stockist.EndDate.Should().Be(ArtistsNewViewModelTests.EndDate);
            }
        }

        public class StatusesTests : ArtistsNewViewModelTests
        {
            [Fact]
            public void ShouldHaveCorrectAvailableOptions()
            {
                var statuses = this.Subject.Statuses;
                statuses.Should().BeEquivalentTo(StatusMode.Inactive, StatusMode.ActiveHidden, StatusMode.Active);
            }
        }

        public class SaveTests : ArtistsNewViewModelTests
        {
            private readonly ValidationResult failure;
            private readonly ValidationResult success;

            public SaveTests()
            {
                this.failure = new ValidationResult(this.fixture.CreateMany<ValidationFailure>());
                this.success = new ValidationResult();
            }

            [Fact]
            public async Task ShouldUpdateValidationResultOnSaving()
            {
                this.GivenValidationResult(this.failure);
                var subject = this.Subject;

                await subject.Save.Execute();
                subject.ValidationResult.Should().Be(this.failure);
            }

            [Fact]
            public async Task ShouldNotCallServiceIfValidationFails()
            {
                this.GivenValidationResult(this.failure);
                await this.Subject.Save.Execute();
                this.stockistService.Verify(x => x.SaveStockistAsync(It.IsAny<Stockist>()), Times.Never());
            }

            [Fact]
            public async Task ShouldCallServiceOnSuccessfulValidation()
            {
                this.GivenValidationResult(this.success);
                await this.Subject.Save.Execute();
                this.stockistService.Verify(x => x.SaveStockistAsync(It.IsAny<Stockist>()), Times.Once());
            }

            [Fact]
            public async Task ShouldNavigateToEditNewArtistOnSuccess()
            {
                this.GivenValidationResult(this.success);
                var subject = this.Subject;

                subject.Stockist.StockistCode = "TLM";
                await subject.Save.Execute();

                this.navigationManager.Uri.Should().EndWith("/artists/edit/TLM");
            }
        }

        public class CancelTests : ArtistsNewViewModelTests
        {
            [Fact]
            public async Task ShouldRedirectToIndexPageOnCancelling()
            {
                await this.Subject.Cancel.Execute();
                this.navigationManager.Uri.Should().EndWith("/artists");
            }
        }
    }
}
