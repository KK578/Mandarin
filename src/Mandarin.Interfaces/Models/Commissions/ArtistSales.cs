using System;
using System.Collections.Generic;
using Mandarin.Converters;
using Newtonsoft.Json;

namespace Mandarin.Models.Commissions
{
    public class ArtistSales
    {
        public ArtistSales(string stockistCode,
                           string name,
                           string emailAddress,
                           string customMessage,
                           DateTime startDate,
                           DateTime endDate,
                           decimal rate,
                           List<Sale> sales,
                           decimal subtotal,
                           decimal commissionTotal,
                           decimal total)
        {
            this.StockistCode = stockistCode;
            this.Name = name;
            this.EmailAddress = emailAddress;
            this.CustomMessage = customMessage;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Rate = rate;
            this.Sales = sales?.AsReadOnly();
            this.Subtotal = subtotal;
            this.CommissionTotal = commissionTotal;
            this.Total = total;
        }

        [JsonProperty("stockistCode")] public string StockistCode { get; }
        [JsonProperty("name")] public string Name { get; }
        [JsonProperty("emailAddress")] public string EmailAddress { get; }
        [JsonProperty("customMessage")] public string CustomMessage { get; }

        [JsonProperty("startDate"), JsonConverter(typeof(IsoDateConverter))] public DateTime StartDate { get; }
        [JsonProperty("endDate"), JsonConverter(typeof(IsoDateConverter))] public DateTime EndDate { get; }
        [JsonProperty("rate"), JsonConverter(typeof(DoubleAsPercentageConverter))] public decimal Rate { get; }

        [JsonProperty("sales")] public IEnumerable<Sale> Sales { get; }
        [JsonProperty("subtotal"), JsonConverter(typeof(DoubleAsCurrencyConverter))] public decimal Subtotal { get; }
        [JsonProperty("commissionTotal"), JsonConverter(typeof(DoubleAsCurrencyConverter))] public decimal CommissionTotal { get; }
        [JsonProperty("total"), JsonConverter(typeof(DoubleAsCurrencyConverter))] public decimal Total { get; }

        public override string ToString()
        {
            return $"{this.StockistCode}: {this.Subtotal:C} @ {this.Rate:P} = {this.CommissionTotal:C}|{this.Total:C}" ;
        }
    }
}
