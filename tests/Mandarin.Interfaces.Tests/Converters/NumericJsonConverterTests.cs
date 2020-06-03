using System.Collections.Generic;
using Bashi.Tests.Framework.Data;
using Mandarin.Converters;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Converters
{
    [TestFixture]
    public class NumericJsonConverterTests
    {
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

        private static IEnumerable<object> NumericValues
        {
            get
            {
                yield return TestData.Create<int>();
                yield return TestData.Create<short>();
                yield return TestData.Create<long>();
                yield return TestData.Create<float>();
                yield return TestData.Create<double>();
                yield return TestData.Create<decimal>();
            }
        }
    }
}
