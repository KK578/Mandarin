using System;
using Bashi.Tests.Framework.Data;
using Mandarin.Converters;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Converters
{
    [TestFixture]
    public class IsoDateConverterTests
    {
        [Test]
        public void ConvertToDate_ValueShouldBeFormattedAsExpected()
        {
            var dateTime = new DateTime(2020, 06, 03);
            var subject = new IsoDateConverter();
            var value = JsonConverterTestHelper.WriteToString(subject, dateTime);
            Assert.That(value, Is.EqualTo("\"03/06/2020\""));
        }

        [Test]
        public void ConvertToDate_ValueShouldSuccessfullyRoundTrip()
        {
            var dateTime = TestData.Create<DateTime>().Date;
            var subject = new IsoDateConverter();
            JsonConverterTestHelper.AssertCanConvert(subject, dateTime);
            JsonConverterTestHelper.AssertRoundTripOfValue(subject, dateTime);
        }
    }
}
