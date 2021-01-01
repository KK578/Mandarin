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
                var recordOfSales = new RecordOfSales(TestData.NextString(),
                                                      TestData.NextString(),
                                                      TestData.NextString(),
                                                      TestData.NextString(),
                                                      TestData.NextString(),
                                                      TestData.Create<DateTime>(),
                                                      TestData.Create<DateTime>(),
                                                      TestData.Create<decimal>(),
                                                      null,
                                                      TestData.Create<decimal>(),
                                                      TestData.Create<decimal>(),
                                                      TestData.Create<decimal>());

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
                var data = new RecordOfSales("TLM",
                                             "Owner",
                                             "The Little Mandarin",
                                             "email@address.com",
                                             "My Message",
                                             new DateTime(2020, 06, 01),
                                             new DateTime(2020, 06, 03),
                                             0.10m,
                                             new List<Sale>
                                             {
                                                 new("TLM-001", "A Mandarin", 1, 2.00M, 2.00M, -0.20M, 1.80M),
                                                 new("TLM-002", "An Orange", 2, 4.00M, 8.00M, -0.80M, 9.20M),
                                             },
                                             10.00M,
                                             -1.00M,
                                             9.00M);

                var snapshot = await File.ReadAllTextAsync(WellKnownTestData.Commissions.RecordOfSalesTLM);
                var actual = JsonConvert.SerializeObject(data, Formatting.Indented);

                actual.Should().BeEquivalentTo(snapshot.Trim());
            }
        }
    }
}
