using Bashi.Tests.Framework.Data;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using Moq;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests
{
    [TestFixture]
    public class ViewModelFactoryTests
    {
        [Test]
        public void CreateArtistRecordOfSalesViewModel_CanCreate()
        {
            var subject = new ViewModelFactory(Mock.Of<IEmailService>());
            Assert.That(() => subject.CreateArtistRecordOfSalesViewModel(TestData.Create<ArtistSales>()),
                        Throws.Nothing);
        }
    }
}
