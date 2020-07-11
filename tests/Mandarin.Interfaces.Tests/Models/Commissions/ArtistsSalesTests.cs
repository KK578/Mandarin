using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bashi.Tests.Framework.Data;
using Mandarin.Models.Commissions;
using Mandarin.Tests.Data;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Models.Commissions
{
    [TestFixture]
    public class ArtistsSalesTests
    {
        [Test]
        public void WithMessageCustomisations_MaintainsNullSalesList()
        {
            var artistSales = new ArtistSales(TestData.NextString(),
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

            var copy = artistSales.WithMessageCustomisations(null, null);
            Assert.That(copy.Sales, Is.Null);
        }

        [Test]
        public void WithMessageCustomisations_EmailAndMessageShouldUpdateWhenNotNull()
        {
            var original = TestData.Create<ArtistSales>();

            var copyWithNulls = original.WithMessageCustomisations(null, null);
            Assert.That(copyWithNulls.EmailAddress, Is.EqualTo(original.EmailAddress));
            Assert.That(copyWithNulls.CustomMessage, Is.EqualTo(original.CustomMessage));

            var email = TestData.NextString();
            var message = TestData.NextString();
            var updated = original.WithMessageCustomisations(email, message);
            Assert.That(updated.EmailAddress, Is.EqualTo(email));
            Assert.That(updated.CustomMessage, Is.EqualTo(message));
        }

        [Test]
        public async Task AsJson_ShouldMatchSnapshot()
        {
            var data = new ArtistSales("TLM",
                                       "The",
                                       "Little Mandarin",
                                       "email@address.com",
                                       "My Message",
                                       new DateTime(2020, 06, 01),
                                       new DateTime(2020, 06, 03),
                                       0.10m,
                                       new List<Sale>
                                       {
                                           new Sale("TLM-001", "A Mandarin", 1, 2.00M, 2.00M, -0.20M, 1.80M),
                                           new Sale("TLM-002", "An Orange", 2, 4.00M, 8.00M, -0.80M, 9.20M),
                                       },
                                       10.00M,
                                       -1.00M,
                                       9.00M);

            var snapshot = await File.ReadAllTextAsync(WellKnownTestData.Commissions.ArtistSalesTLM);
            var actual = JsonConvert.SerializeObject(data, Formatting.Indented);

            Assert.That(ArtistsSalesTests.SanitizeWhitespace(actual),
                        Is.EqualTo(ArtistsSalesTests.SanitizeWhitespace(snapshot)).NoClip.AsCollection);
        }

        private static string SanitizeWhitespace(string input)
        {
            var trimmed = input.Replace("\r", string.Empty).Replace("\n", string.Empty).Split(" ", StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", trimmed);
        }
    }
}
