using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Converters
{
    public static class JsonConverterTestHelper
    {
        private static readonly JsonSerializer JsonSerializer = JsonSerializer.CreateDefault();

        public static void AssertRoundTripOfValue(JsonConverter subject, object o)
        {
            var serialized = JsonConverterTestHelper.WriteToString(subject, o);
            using var sr = new StringReader(serialized);
            using var jsonReader = new JsonTextReader(sr);
            jsonReader.Read();
            var data = subject.ReadJson(jsonReader, o.GetType(), null, JsonConverterTestHelper.JsonSerializer);

            Assert.That(data, Is.EqualTo(o));
        }

        public static string WriteToString(JsonConverter subject, object o)
        {
            var sw = new StringWriter();
            using var jsonWriter = new JsonTextWriter(sw);
            subject.WriteJson(jsonWriter, o, JsonConverterTestHelper.JsonSerializer);

            var serialized = sw.ToString();
            return serialized;
        }

        public static void AssertCanConvert(JsonConverter subject, object o)
        {
            Assert.That(subject.CanConvert(o.GetType()), Is.True);
        }
    }
}
