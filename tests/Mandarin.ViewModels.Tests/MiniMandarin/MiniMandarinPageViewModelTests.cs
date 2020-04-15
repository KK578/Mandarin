using Mandarin.ViewModels.MiniMandarin;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.MiniMandarin
{
    [TestFixture]
    public class MiniMandarinPageViewModelTests
    {
        [Test]
        public void Paragraphs_ShouldContainASingleParagraph()
        {
            var subject = new MiniMandarinPageViewModel();
            Assert.That(subject.Paragraphs, Has.Count.EqualTo(1));
        }

        [Test]
        public void BannerImageViewModel_ShouldPointToBanner()
        {
            var subject = new MiniMandarinPageViewModel();
            Assert.That(subject.BannerImageViewModel.SourceUrl.ToString(), Contains.Substring("Banner"));
            Assert.That(subject.BannerImageViewModel.Description, Contains.Substring("Bearcarons"));
        }

        [Test]
        public void MacaronImageViewModels_ShouldHave3Images()
        {
            var subject = new MiniMandarinPageViewModel();
            Assert.That(subject.MacaronImageViewModels, Has.Count.EqualTo(3));
        }
    }
}
