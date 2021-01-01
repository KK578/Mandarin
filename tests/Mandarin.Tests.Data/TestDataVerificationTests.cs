using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Mandarin.Tests.Data
{
    public class TestDataVerificationTests
    {
        public static IEnumerable<object[]> ListAllFileNames()
        {
            var assemblyTypes = typeof(TestDataVerificationTests).Assembly.GetTypes();
            var staticTypes = assemblyTypes.Where(t => t.IsClass && t.IsSealed && t.IsAbstract);
            var constFields = staticTypes.SelectMany(x => x.GetFields(BindingFlags.Public | BindingFlags.Static));
            foreach (var constantField in constFields)
            {
                var value = constantField.GetValue(null);
                if (value is string s)
                {
                    yield return new object[] { s };
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestDataVerificationTests.ListAllFileNames))]
        public void VerifyFileExists(string filename)
        {
            File.Exists(filename).Should().BeTrue();
        }
    }
}
