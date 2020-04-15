using Mandarin.ViewModels.Index.OpeningTimes;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index.OpeningTimes
{
    [TestFixture]
    public class OpeningTimesViewModelTests
    {
        [Test]
        public void OpeningTimes_ShouldHave7Dates()
        {
            var subject = new OpeningTimesViewModel();
            Assert.That(subject.OpeningTimes, Has.Count.EqualTo(7));
        }
    }
}
