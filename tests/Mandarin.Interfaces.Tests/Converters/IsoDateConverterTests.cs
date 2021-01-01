using System;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Converters;
using Xunit;

namespace Mandarin.Interfaces.Tests.Converters
{
    public class IsoDateConverterTests
    {
        [Fact]
        public void ConvertToDate_ValueShouldBeFormattedAsExpected()
        {
            var dateTime = new DateTime(2020, 06, 03);
            var subject = new IsoDateConverter();
            var value = JsonConverterTestHelper.WriteToString(subject, dateTime);

            value.Should().Be("\"03/06/2020\"");
        }

        [Fact]
        public void ConvertToDate_ValueShouldSuccessfullyRoundTrip()
        {
            var dateTime = TestData.Create<DateTime>().Date;
            var subject = new IsoDateConverter();
            JsonConverterTestHelper.AssertCanConvert(subject, dateTime);
            JsonConverterTestHelper.AssertRoundTripOfValue(subject, dateTime);
        }
    }
}
