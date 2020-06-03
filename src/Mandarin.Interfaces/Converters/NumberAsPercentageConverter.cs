using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Mandarin.Converters
{
    public class NumberAsPercentageConverter : JsonConverter
    {
        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo("en-GB");

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stringValue = value switch
            {
                short s => s.ToString("P2", NumberAsPercentageConverter.CultureInfo),
                int i => i.ToString("P2", NumberAsPercentageConverter.CultureInfo),
                long l => l.ToString("P2", NumberAsPercentageConverter.CultureInfo),
                float f => f.ToString("P2", NumberAsPercentageConverter.CultureInfo),
                double d => d.ToString("P2", NumberAsPercentageConverter.CultureInfo),
                decimal d => d.ToString("P2", NumberAsPercentageConverter.CultureInfo),
                null => null,
                _ => throw new ArgumentException($"Cannot format type {value.GetType()} to percentage."),
            };
            writer.WriteValue(stringValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var s = (string)reader.Value;
            var numberValue = double.Parse(s.TrimEnd('%'), NumberAsPercentageConverter.CultureInfo) / 100;
            return Convert.ChangeType(numberValue, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(short)
                   || objectType == typeof(int)
                   || objectType == typeof(long)
                   || objectType == typeof(float)
                   || objectType == typeof(double)
                   || objectType == typeof(decimal);
        }
    }
}
