using FluentAssertions;
using Mandarin.Emails;
using Mandarin.Tests.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests
{
    public class ViewModelFactoryTests
    {
        [Fact]
        public void CreateArtistRecordOfSalesViewModel_CanCreate()
        {
            var subject = new ViewModelFactory(Mock.Of<IEmailService>(), Mock.Of<AuthenticationStateProvider>());
            subject.Invoking(x => x.CreateArtistRecordOfSalesViewModel(MandarinFixture.Instance.NewRecordOfSales))
                   .Should()
                   .NotThrow();
        }
    }
}
