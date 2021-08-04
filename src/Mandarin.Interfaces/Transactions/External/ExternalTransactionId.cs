using Bashi.Core.TinyTypes;
using Newtonsoft.Json;

namespace Mandarin.Transactions.External
{
    /// <summary>
    /// Represents the unique Square Id for a <see cref="Transaction"/>.
    /// </summary>
    public class ExternalTransactionId : TinyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalTransactionId"/> class.
        /// </summary>
        /// <param name="value">The unique Transaction Id.</param>
        [JsonConstructor]
        private ExternalTransactionId(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalTransactionId"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique External Transaction Id.</param>
        /// <returns>A newly created <see cref="ExternalTransactionId"/> or null/empty.</returns>
        public static ExternalTransactionId Of(string value) => string.IsNullOrEmpty(value) ? null : new ExternalTransactionId(value);
    }
}
