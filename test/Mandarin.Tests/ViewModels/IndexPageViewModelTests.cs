using Mandarin.ViewModels;
using NUnit.Framework;

namespace Mandarin.Tests.ViewModels
{
    [TestFixture]
    public class IndexPageViewModelTests
    {
        [Test]
        public void Paragraphs_ShouldContain4Paragraphs()
        {
            var subject = new IndexPageViewModel();
            Assert.That(subject.Paragraphs, Has.Count.EqualTo(4));
        }
    }
}
