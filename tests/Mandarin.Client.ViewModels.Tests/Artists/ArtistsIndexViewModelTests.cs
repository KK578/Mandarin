using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Client.ViewModels.Artists;
using Mandarin.Client.ViewModels.Tests.Helpers;
using Mandarin.Stockists;
using Mandarin.Tests.Data;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Artists
{
    public class ArtistsIndexViewModelTests
    {
        private readonly Mock<IStockistService> stockistService = new();
        private readonly MockNavigationManager navigationManager = new();

        private IArtistsIndexViewModel Subject =>
            new ArtistsIndexViewModel(this.stockistService.Object, this.navigationManager);

        public class IsLoadingTests : ArtistsIndexViewModelTests
        {
            [Fact]
            public void ShouldBeFalseOnConstruction()
            {
                this.Subject.IsLoading.Should().BeFalse();
            }

            [Fact]
            public void ShouldBeTrueWhilstExecuting()
            {
                var tcs = new TaskCompletionSource<IReadOnlyList<Stockist>>();
                this.stockistService.Setup(x => x.GetStockistsAsync()).Returns(tcs.Task);

                var subject = this.Subject;
                var unused = subject.LoadData.Execute().ToTask();

                subject.IsLoading.Should().BeTrue();
                tcs.SetCanceled();
            }

            [Fact]
            public async Task ShouldBeFalseAfterLoadingFinishes()
            {
                this.stockistService.Setup(x => x.GetStockistsAsync()).ReturnsAsync(new List<Stockist>());

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.IsLoading.Should().BeFalse();
            }
        }

        public class RowsTests : ArtistsIndexViewModelTests
        {
            [Fact]
            public void ShouldNotBeNullOnConstruction()
            {
                this.Subject.Rows.Should().NotBeNull();
            }

            [Fact]
            public async Task ShouldBePresentAfterLoadDataIsExecuted()
            {
                var data = new List<Stockist> { WellKnownTestData.Stockists.KelbyTynan };
                this.stockistService.Setup(x => x.GetStockistsAsync()).ReturnsAsync(data);

                var subject = this.Subject;
                await subject.LoadData.Execute();

                subject.Rows.Should().HaveCount(1);
            }
        }

        public class CreateNewTests : ArtistsIndexViewModelTests
        {
            [Fact]
            public async Task ShouldBeExecutableOnConstruction()
            {
                var result = await this.Subject.CreateNew.CanExecute.FirstAsync();
                result.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldRedirectOnExecute()
            {
                await this.Subject.CreateNew.Execute();
                this.navigationManager.Uri.Should().Contain("/artists/new");
            }
        }

        public class EditSelectedTests : ArtistsIndexViewModelTests
        {
            [Fact]
            public async Task ShouldNotBeExecutableOnConstruction()
            {
                var result = await this.Subject.EditSelected.CanExecute.FirstAsync();
                result.Should().BeFalse();
            }

            [Fact]
            public async Task ShouldBeExecutableOnceARowIsSelected()
            {
                var subject = this.Subject;
                subject.SelectedRow = Mock.Of<IArtistViewModel>();
                var result = await subject.EditSelected.CanExecute.FirstAsync();
                result.Should().BeTrue();
            }

            [Fact]
            public async Task ShouldRedirectOnExecute()
            {
                var subject = this.Subject;
                subject.SelectedRow = Mock.Of<IArtistViewModel>(x => x.StockistCode == "TLM");
                await subject.EditSelected.Execute();
                this.navigationManager.Uri.Should().Contain("/artists/edit/TLM");
            }
        }
    }
}
