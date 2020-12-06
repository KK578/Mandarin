using Bashi.Tests.Framework.Data;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests
{
    [TestFixture]
    public class ViewModelFactoryTests
    {
        [Test]
        public void CreateArtistRecordOfSalesViewModel_CanCreate()
        {
            var pageContentModel = new PageContentModel(JToken.FromObject(new object()));
            var subject = new ViewModelFactory(Mock.Of<IEmailService>(), pageContentModel, Mock.Of<IHttpContextAccessor>());
            Assert.That(() => subject.CreateArtistRecordOfSalesViewModel(TestData.Create<ArtistSales>()), Throws.Nothing);
        }
    }
}
