using Mandarin.Services.Email;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Models.Contact
{
    [TestFixture]
    public class EmailResponseTests
    {
        [Test]
        [TestCase(101, false)]
        [TestCase(200, true)]
        [TestCase(204, true)]
        [TestCase(304, true)]
        [TestCase(401, false)]
        public void IsSuccess_ReflectingSuccessfulHttpStatusCodes(int statusCode, bool expected)
        {
            Assert.That(new EmailResponse(statusCode).IsSuccess, Is.EqualTo(expected));
        }
    }
}
