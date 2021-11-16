using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Blazorise.DataGrid;
using Bunit;
using FluentAssertions;
using Mandarin.Client.Components.Animations;
using Mandarin.Client.Components.Reactive;
using Mandarin.Client.Pages.Artists;
using Mandarin.Client.Tests.Helpers;
using Mandarin.Client.ViewModels.Artists;
using Mandarin.Tests.Data;
using NodaTime;
using ReactiveUI;
using Xunit;

namespace Mandarin.Client.Tests.Pages.Artists
{
    public sealed class ArtistsIndexTests : MandarinTestContext<ArtistsIndex, IArtistsIndexViewModel>
    {
        private readonly ObservableCollection<IArtistViewModel> artists = new();

        public ArtistsIndexTests()
        {
            this.ViewModel.Setup(x => x.Rows).Returns(new ReadOnlyObservableCollection<IArtistViewModel>(this.artists));
            this.ViewModel.Setup(x => x.LoadData).Returns(ReactiveCommand.Create(GetArtists));
            this.ViewModel.Setup(x => x.CreateNew).Returns(ReactiveCommand.Create(DoNothing));
            this.ViewModel.Setup(x => x.EditSelected).Returns(ReactiveCommand.Create(DoNothing));

            IReadOnlyCollection<IArtistViewModel> GetArtists() => this.artists;

            static void DoNothing()
            {
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldDisplayProgressBarWhilstLoading(bool isLoading)
        {
            this.ViewModel.Setup(x => x.IsLoading).Returns(isLoading);
            this.Subject.HasComponent<MandarinProgressBar>().Should().Be(isLoading);
        }

        [Fact]
        public void ShouldHaveButtonsBoundToCorrectCommands()
        {
            var buttons = this.Subject.FindComponents<ReactiveButton<Unit, Unit>>();
            buttons.Should().HaveCount(2);

            buttons[0].Instance.ReactiveCommand.Should().Be(this.ViewModel.Object.EditSelected);
            buttons[0].Markup.Should().Contain("Edit");

            buttons[1].Instance.ReactiveCommand.Should().Be(this.ViewModel.Object.CreateNew);
            buttons[1].Markup.Should().Contain("New");
        }

        [Fact]
        public void ShouldBindDataGridRowsToViewModel()
        {
            this.artists.Add(new ArtistViewModel(WellKnownTestData.Stockists.TheLittleMandarin, SystemClock.Instance));
            this.artists.Add(new ArtistViewModel(WellKnownTestData.Stockists.ArlueneWoodes, SystemClock.Instance));

            var dataGrid = this.Subject.FindComponent<DataGrid<IArtistViewModel>>();

            var tableHeaders = dataGrid.FindAll("thead > tr:first-child > th");
            tableHeaders.Select(x => x.TextContent).Should().BeEquivalentTo("Code", "Name", "Status");

            var cells = dataGrid.FindAll("tbody > tr > td");
            cells.Select(x => x.TextContent).Should().BeEquivalentTo("TLM", "The Little Mandarin Team", "Active", "AW20", "Arluene Woodes", "Active");
        }

        [Fact]
        public void ShouldPushSelectedRowToViewModel()
        {
            this.ViewModel.SetupProperty(x => x.SelectedRow);
            this.artists.Add(new ArtistViewModel(WellKnownTestData.Stockists.TheLittleMandarin, SystemClock.Instance));

            var row = this.Subject.Find("tbody > tr");
            row.Click();

            this.ViewModel.Object.SelectedRow.Should().Be(this.artists[0]);
        }
    }
}
