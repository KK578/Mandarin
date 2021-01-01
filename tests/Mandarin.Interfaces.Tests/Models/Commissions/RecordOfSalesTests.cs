using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using FluentAssertions;
using Mandarin.Models.Commissions;
using Mandarin.Tests.Data;
using Newtonsoft.Json;
using Xunit;

namespace Mandarin.Interfaces.Tests.Models.Commissions
{
    public class RecordOfSalesTests
    {
        public class WithMessageCustomisationsTests : RecordOfSalesTests
        {
            [Fact]
            public void ShouldMaintainNullSalesList()
            {
                var recordOfSales = TestData.Create<RecordOfSales>() with
                {
                    Sales = null,
                };

                var copy = recordOfSales.WithMessageCustomisations(null, null);
                copy.Sales.Should().BeNull();
            }

            [Fact]
            public void WithMessageCustomisations_EmailAndMessageShouldUpdateWhenNotNull()
            {
                var original = TestData.Create<RecordOfSales>();

                var copyWithNulls = original.WithMessageCustomisations(null, null);
                copyWithNulls.EmailAddress.Should().Be(original.EmailAddress);
                copyWithNulls.CustomMessage.Should().Be(original.CustomMessage);

                var email = TestData.NextString();
                var message = TestData.NextString();
                var updated = original.WithMessageCustomisations(email, message);
                updated.EmailAddress.Should().Be(email);
                updated.CustomMessage.Should().Be(message);
            }
        }

        public class AsJsonTests : RecordOfSalesTests
        {
            [Fact]
            public async Task AsJson_ShouldMatchSnapshot()
            {
                var data = new RecordOfSales
                {
                    StockistCode = "TLM",
                    FirstName = "Owner",
                    Name = "The Little Mandarin",
                    EmailAddress = "email@address.com",
                    CustomMessage = "My Message",
                    StartDate = new DateTime(2020, 06, 01),
                    EndDate = new DateTime(2020, 06, 03),
                    Rate = 0.10m,
                    Sales = new List<Sale>
                    {
                        new()
                        {
                            ProductCode = "TLM-001",
                            ProductName = "A Mandarin",
                            Quantity = 1,
                            UnitPrice = 2.00M,
                            Subtotal = 2.00M,
                            Commission = -0.20M,
                            Total = 1.80M,
                        },
                        new()
                        {
                            ProductCode = "TLM-002",
                            ProductName = "An Orange",
                            Quantity = 2,
                            UnitPrice = 4.00M,
                            Subtotal = 8.00M,
                            Commission = -0.80M,
                            Total = 9.20M,
                        },
                    }.AsReadOnly(),
                    Subtotal = 10.00M,
                    CommissionTotal = -1.00M,
                    Total = 9.00M,
                };

                var snapshot = await File.ReadAllTextAsync(WellKnownTestData.Commissions.RecordOfSalesTLM);
                var actual = JsonConvert.SerializeObject(data, Formatting.Indented);

                actual.Should().BeEquivalentTo(snapshot.Trim());
            }
        }
    }
}
