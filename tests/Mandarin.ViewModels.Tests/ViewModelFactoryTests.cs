using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Emails;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using Xunit;

namespace Mandarin.ViewModels.Tests
{
    public class ViewModelFactoryTests
    {
        [Fact]
        public void CreateArtistRecordOfSalesViewModel_CanCreate()
        {
            var subject = new ViewModelFactory(Mock.Of<IEmailService>(), Mock.Of<AuthenticationStateProvider>());
            subject.Invoking(x => x.CreateArtistRecordOfSalesViewModel(TestData.Create<RecordOfSales>()))
                   .Should()
                   .NotThrow();
        }
    }
}
