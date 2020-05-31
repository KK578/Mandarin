using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Mandarin.Converters
{
    public class DoubleAsCurrencyConverter : JsonConverter<double>
    {
        public override void WriteJson(JsonWriter writer, double value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("C", CultureInfo.GetCultureInfo("en-GB")));
        }

        public override double ReadJson(JsonReader reader,
                                        Type objectType,
                                        double existingValue,
                                        bool hasExistingValue,
                                        JsonSerializer serializer)
        {
            var s = (string) reader.Value;
            return double.Parse(s, NumberStyles.Currency, CultureInfo.GetCultureInfo("en-GB"));
        }
    }
}
