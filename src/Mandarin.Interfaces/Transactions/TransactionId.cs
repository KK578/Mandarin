using Bashi.Core.TinyTypes;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents the unique Id for a <see cref="Transaction"/>.
    /// </summary>
    public class TransactionId : TinyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionId"/> class.
        /// </summary>
        /// <param name="value">The unique Transaction Id.</param>
        private TransactionId(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionId"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique Transaction Id.</param>
        /// <returns>A newly created <see cref="TransactionId"/> or null/empty.</returns>
        public static TransactionId Of(string value) => string.IsNullOrEmpty(value) ? null : new TransactionId(value);
    }
}
