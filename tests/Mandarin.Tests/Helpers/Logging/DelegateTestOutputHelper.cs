using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mandarin.Tests.Helpers.Logging
{
    /// <summary>
    /// Represents a class which writes test output to a nested <see cref="ITestOutputHelper"/>.
    /// This allows the same test output to be passed to Serilog, with the underlying test logger changing.
    /// </summary>
    internal class DelegateTestOutputHelper : ITestOutputHelper
    {
        public ITestOutputHelper TestOutputHelper { get; set; } = new TestOutputHelper();

        /// <inheritdoc />
        public void WriteLine(string message)
        {
            this.TestOutputHelper.WriteLine(message);
        }

        /// <inheritdoc />
        public void WriteLine(string format, params object[] args)
        {
            this.TestOutputHelper.WriteLine(format, args);
        }
    }
}
