using System.Collections.Generic;
using System.IO;
using Mandarin.Converters;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Converters
{
    [TestFixture]
    public class NumericJsonConverterTests
    {
        private static readonly JsonSerializer JsonSerializer = JsonSerializer.CreateDefault();

        [Test]
        [TestCaseSource(nameof(NumericJsonConverterTests.NumericValues))]
        public void ConvertToPercentage_ValueShouldSuccessfullyRoundTrip(object o)
        {
            var subject = new NumberAsPercentageConverter();
            NumericJsonConverterTests.AssertCanConvert(subject, o);
            NumericJsonConverterTests.AssertRoundTripOfValue(subject, o);
        }

        [Test]
        [TestCaseSource(nameof(NumericJsonConverterTests.NumericValues))]
        public void ConvertToCurrency_ValueShouldSuccessfullyRoundTrip(object o)
        {
            var subject = new NumberAsCurrencyConverter();
            NumericJsonConverterTests.AssertCanConvert(subject, o);
            NumericJsonConverterTests.AssertRoundTripOfValue(subject, o);
        }

        private static void AssertRoundTripOfValue(JsonConverter subject, object o)
        {
            var sw = new StringWriter();
            using var jsonWriter = new JsonTextWriter(sw);
            subject.WriteJson(jsonWriter, o, NumericJsonConverterTests.JsonSerializer);

            var serialized = sw.ToString();
            using var sr = new StringReader(serialized);
            using var jsonReader = new JsonTextReader(sr);
            var data = subject.ReadJson(jsonReader, o.GetType(), null, NumericJsonConverterTests.JsonSerializer);

            Assert.That(data, Is.EqualTo(o));
        }

        private static void AssertCanConvert(JsonConverter subject, object o)
        {
            Assert.That(subject.CanConvert(o.GetType()), Is.True);
        }

        private static IEnumerable<object> NumericValues
        {
            get
            {
                yield return (int)1223423434;
                yield return (short)234;
                yield return (long)2123123123123123l;
                yield return (float)6.1f;
                yield return (double)10.02d;
                yield return (decimal)0.34m;
            }
        }
    }
}
