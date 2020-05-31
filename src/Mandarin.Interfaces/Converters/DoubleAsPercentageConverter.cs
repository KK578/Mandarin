using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Mandarin.Converters
{
    public class DoubleAsPercentageConverter : JsonConverter<double>
    {
        public override void WriteJson(JsonWriter writer, double value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("P2", CultureInfo.GetCultureInfo("en-GB")));
        }

        public override double ReadJson(JsonReader reader,
                                        Type objectType,
                                        double existingValue,
                                        bool hasExistingValue,
                                        JsonSerializer serializer)
        {
            var s = (string) reader.Value;
            return double.Parse(s.TrimEnd('%'), CultureInfo.GetCultureInfo("en-GB")) / 100;
        }
    }
}
