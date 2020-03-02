using System;
using Mandarin.ViewModels.Index.OpeningTimes;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index.OpeningTimes
{
    [TestFixture]
    public class OpeningTimeRowViewModelTests
    {
        [Test]
        public void Properties_ShouldMatchConstructorInputs()
        {
            var subject = new OpeningTimeRowViewModel("Monday", "Closed");
            Assert.That(subject.NameOfDay, Is.EqualTo("Monday"));
            Assert.That(subject.Message, Is.EqualTo("Closed"));
        }

        [Test]
        public void Message_GivenTwoDateTimes_ShouldShowFormattedTimes()
        {
            var openTime = new DateTime(2020, 1, 1, 09, 00, 00, DateTimeKind.Utc);
            var closeTime = new DateTime(2020, 1, 1, 17, 00, 00, DateTimeKind.Utc);
            var subject = new OpeningTimeRowViewModel("Monday", openTime, closeTime);
            Assert.That(subject.Message, Is.EqualTo("09:00 - 17:00"));
        }
    }
}
