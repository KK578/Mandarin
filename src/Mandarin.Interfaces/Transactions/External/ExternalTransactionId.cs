using Bashi.Core.TinyTypes;

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
        public ExternalTransactionId(string value)
            : base(value)
        {
        }
    }
}
