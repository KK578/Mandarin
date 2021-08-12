using System;
using AutoFixture;
using Mandarin.Commissions;
using Mandarin.Inventory;
using Mandarin.Tests.Data.Extensions;
using Mandarin.Transactions;
using MicroElements.AutoFixture.NodaTime;
using NodaTime;

namespace Mandarin.Tests.Data
{
    public sealed class MandarinFixture : Fixture
    {
        public static readonly MandarinFixture Instance = new();

        public MandarinFixture()
        {
            this.Customize(new NodaTimeCustomization());
        }

        public Exception NewException => this.Create<Exception>();

        public Instant NewInstant => this.Create<Instant>();

        public Product NewProduct => this.Create<Product>();

        public Product NewProductTlm => this.NewProduct.WithTlmProductCode();

        public RecordOfSales NewRecordOfSales => this.Create<RecordOfSales>();

        public string NewString => this.Create<string>();

        public Transaction NewTransaction => this.Create<Transaction>();
    }
}
