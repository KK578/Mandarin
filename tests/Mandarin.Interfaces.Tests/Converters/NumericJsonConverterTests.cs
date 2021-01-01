using System.Collections.Generic;
using Mandarin.Converters;
using Xunit;

namespace Mandarin.Interfaces.Tests.Converters
{
    public class NumericJsonConverterTests
    {
        public static IEnumerable<object[]> NumericValues
        {
            get
            {
                yield return new object[] { (int)1234 };
                yield return new object[] { (short)1234 };
                yield return new object[] { 1234L };
                yield return new object[] { 1234.5F };
                yield return new object[] { 1234.5D };
                yield return new object[] { 1234.5M };
            }
        }

        [Theory]
        [MemberData(nameof(NumericJsonConverterTests.NumericValues))]
        public void ConvertToPercentage_ValueShouldSuccessfullyRoundTrip(object o)
        {
            var subject = new NumberAsPercentageConverter();
            JsonConverterTestHelper.AssertCanConvert(subject, o);
            JsonConverterTestHelper.AssertRoundTripOfValue(subject, o);
        }

        [Theory]
        [MemberData(nameof(NumericJsonConverterTests.NumericValues))]
        public void ConvertToCurrency_ValueShouldSuccessfullyRoundTrip(object o)
        {
            var subject = new NumberAsCurrencyConverter();
            JsonConverterTestHelper.AssertCanConvert(subject, o);
            JsonConverterTestHelper.AssertRoundTripOfValue(subject, o);
        }
    }
}
