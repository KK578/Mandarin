using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Commissions;
using Mandarin.Emails;
using Microsoft.AspNetCore.Components.Authorization;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mandarin.ViewModels.Tests
{
    public class ViewModelFactoryTests
    {
        [Fact]
        public void CreateArtistRecordOfSalesViewModel_CanCreate()
        {
            var pageContentModel = new PageContentModel(JToken.FromObject(new object()));
            var subject = new ViewModelFactory(Mock.Of<IEmailService>(), pageContentModel, Mock.Of<AuthenticationStateProvider>());
            subject.Invoking(x => x.CreateArtistRecordOfSalesViewModel(TestData.Create<RecordOfSales>()))
                   .Should()
                   .NotThrow();
        }
    }
}
