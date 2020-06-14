using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mandarin.ViewModels.Index.OpeningTimes;
using NUnit.Framework;

namespace Mandarin.ViewModels.Tests.Index.OpeningTimes
{
    [TestFixture]
    [SetCulture("en-GB")]
    [SetUICulture("en-GB")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test class clarity.")]
    public class OpeningTimeRowViewModelTests
    {
        private static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                var time9AM = new DateTime(2020, 1, 1, 09, 00, 00, DateTimeKind.Utc);
                var time11AM = new DateTime(2020, 1, 1, 11, 00, 00, DateTimeKind.Utc);
                var time1PM = new DateTime(2020, 1, 1, 13, 00, 00, DateTimeKind.Utc);
                var time5PM = new DateTime(2020, 1, 1, 17, 00, 00, DateTimeKind.Utc);
                var time530PM = new DateTime(2020, 1, 1, 17, 30, 00, DateTimeKind.Utc);

                yield return new TestCaseData(time9AM, time5PM).Returns("9:00am - 5:00pm");
                yield return new TestCaseData(time11AM, time5PM).Returns("11:00am - 5:00pm");
                yield return new TestCaseData(time1PM, time5PM).Returns("1:00pm - 5:00pm");
                yield return new TestCaseData(time1PM, time530PM).Returns("1:00pm - 5:30pm");
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
        [TestCaseSource(nameof(OpeningTimeRowViewModelTests.TestCases))]
        public string Message_GivenTwoDateTimes_ShouldShowFormattedTimes(DateTime openTime, DateTime closeTime)
        {
            var subject = new OpeningTimeRowViewModel("Monday", openTime, closeTime);
            return subject.Message;
        }
    }
}
