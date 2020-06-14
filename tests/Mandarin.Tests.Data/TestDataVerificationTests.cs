using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Mandarin.Tests.Data
{
    [TestFixture]
    public class TestDataVerificationTests
    {
        public static IEnumerable<string> ListAllFileNames()
        {
            var assemblyTypes = typeof(TestDataVerificationTests).Assembly.GetTypes();
            var staticTypes = assemblyTypes.Where(t => t.IsClass && t.IsSealed && t.IsAbstract);
            var constFields = staticTypes.SelectMany(x => x.GetFields(BindingFlags.Public | BindingFlags.Static));
            foreach (var constantField in constFields)
            {
                var value = constantField.GetValue(null);
                if (value is string s)
                {
                    yield return s;
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(TestDataVerificationTests.ListAllFileNames))]
        public void VerifyFileExists(string filename)
        {
            FileAssert.Exists(filename);
        }
    }
}
