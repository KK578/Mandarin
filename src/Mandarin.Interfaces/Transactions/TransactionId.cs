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
        public TransactionId(string value)
            : base(value)
        {
        }
    }
}
