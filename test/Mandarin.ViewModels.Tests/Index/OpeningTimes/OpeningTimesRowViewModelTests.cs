using System;
using System.Collections;
using System.Collections.Generic;
using Mandarin.ViewModels.Index.OpeningTimes;
using NUnit.Framework;
// ReSharper disable InconsistentNaming

namespace Mandarin.ViewModels.Tests.Index.OpeningTimes
{
    [TestFixture]
    [SetCulture("en-GB")]
    [SetUICulture("en-GB")]
    public class OpeningTimeRowViewModelTests
    {
        private static readonly DateTime Time9AM = new DateTime(2020, 1, 1, 09, 00, 00, DateTimeKind.Utc);
        private static readonly DateTime Time11AM = new DateTime(2020, 1, 1, 11, 00, 00, DateTimeKind.Utc);
        private static readonly DateTime Time1PM = new DateTime(2020, 1, 1, 13, 00, 00, DateTimeKind.Utc);
        private static readonly DateTime Time5PM = new DateTime(2020, 1, 1, 17, 00, 00, DateTimeKind.Utc);
        private static readonly DateTime Time530PM = new DateTime(2020, 1, 1, 17, 30, 00, DateTimeKind.Utc);

        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                yield return new TestCaseData(Time9AM, Time5PM).Returns("9:00am - 5:00pm");
                yield return new TestCaseData(Time11AM, Time5PM).Returns("11:00am - 5:00pm");
                yield return new TestCaseData(Time1PM, Time5PM).Returns("1:00pm - 5:00pm");
                yield return new TestCaseData(Time1PM, Time530PM).Returns("1:00pm - 5:30pm");
            }
        }

        [Test]
        public void Properties_ShouldMatchConstructorInputs()
        {
            var subject = new OpeningTimeRowViewModel("Monday", "Closed");
            Assert.That(subject.NameOfDay, Is.EqualTo("Monday"));
            Assert.That(subject.Message, Is.EqualTo("Closed"));
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        public string Message_GivenTwoDateTimes_ShouldShowFormattedTimes(DateTime openTime, DateTime closeTime)
        {
            var subject = new OpeningTimeRowViewModel("Monday", openTime, closeTime);
            return subject.Message;
        }
    }
}
