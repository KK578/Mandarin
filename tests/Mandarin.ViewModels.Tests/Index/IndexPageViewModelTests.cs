using Mandarin.ViewModels.Index;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index
{
    [TestFixture]
    public class IndexPageViewModelTests
    {
        [Test]
        public void MainContent_ShouldBeHtmlString()
        {
            var subject = new IndexPageViewModel(null);
            Assert.That(subject.MainContent.Value, Is.Not.Null);
            Assert.That(subject.MainContent.Value, Contains.Substring("<p>"));
        }
    }
}
