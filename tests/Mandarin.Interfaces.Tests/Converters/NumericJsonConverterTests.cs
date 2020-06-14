using System;
using System.Collections.Generic;
using Mandarin.Converters;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Converters
{
    [TestFixture]
    public class NumericJsonConverterTests
    {
        public static IEnumerable<object> NumericValues
        {
            get
            {
                yield return (int)1234;
                yield return (short)1234;
                yield return 1234L;
                yield return 1234.5F;
                yield return 1234.5D;
                yield return 1234.5M;
            }
        }

        [Test]
        [TestCaseSource(nameof(NumericJsonConverterTests.NumericValues))]
        public void ConvertToPercentage_ValueShouldSuccessfullyRoundTrip(object o)
        {
            var subject = new NumberAsPercentageConverter();
            JsonConverterTestHelper.AssertCanConvert(subject, o);
            JsonConverterTestHelper.AssertRoundTripOfValue(subject, o);
        }

        [Test]
        [TestCaseSource(nameof(NumericJsonConverterTests.NumericValues))]
        public void ConvertToCurrency_ValueShouldSuccessfullyRoundTrip(object o)
        {
            var subject = new NumberAsCurrencyConverter();
            JsonConverterTestHelper.AssertCanConvert(subject, o);
            JsonConverterTestHelper.AssertRoundTripOfValue(subject, o);
        }
    }
}
