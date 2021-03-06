﻿using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Mandarin.Converters
{
    /// <summary>
    /// Implementation of <see cref="JsonConverter"/> that formats a number as a GBP currency string.
    /// </summary>
    public class NumberAsCurrencyConverter : JsonConverter
    {
        private static readonly CultureInfo CultureInfo = CultureInfo.GetCultureInfo("en-GB");

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stringValue = value switch
            {
                short s => s.ToString("C", NumberAsCurrencyConverter.CultureInfo),
                int i => i.ToString("C", NumberAsCurrencyConverter.CultureInfo),
                long l => l.ToString("C", NumberAsCurrencyConverter.CultureInfo),
                float f => f.ToString("C", NumberAsCurrencyConverter.CultureInfo),
                double d => d.ToString("C", NumberAsCurrencyConverter.CultureInfo),
                decimal d => d.ToString("C", NumberAsCurrencyConverter.CultureInfo),
                null => null,
                _ => throw new ArgumentException($"Cannot format type {value.GetType()} to currency."),
            };
            writer.WriteValue(stringValue);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var s = (string)reader.Value;
            var numberValue = double.Parse(s, NumberStyles.Currency, NumberAsCurrencyConverter.CultureInfo);
            return Convert.ChangeType(numberValue, objectType);
        }

        /// <inheritdoc />
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
