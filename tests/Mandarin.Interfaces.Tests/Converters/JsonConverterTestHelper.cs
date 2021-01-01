using System.IO;
using FluentAssertions;
using Newtonsoft.Json;

namespace Mandarin.Interfaces.Tests.Converters
{
    public static class JsonConverterTestHelper
    {
        private static readonly JsonSerializer JsonSerializer = JsonSerializer.CreateDefault();

        public static void AssertRoundTripOfValue(JsonConverter subject, object original)
        {
            var serialized = JsonConverterTestHelper.WriteToString(subject, original);
            using var sr = new StringReader(serialized);
            using var jsonReader = new JsonTextReader(sr);
            jsonReader.Read();
            var data = subject.ReadJson(jsonReader, original.GetType(), null, JsonConverterTestHelper.JsonSerializer);

            data.Should().Be(original);
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
            subject.CanConvert(o.GetType()).Should().BeTrue();
        }
    }
}
