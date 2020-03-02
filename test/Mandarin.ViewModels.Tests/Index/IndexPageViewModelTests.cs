using Mandarin.ViewModels.Index;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index
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
