using Bashi.Core.TinyTypes;

namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents the unique ID for a <see cref="Commission"/>.
    /// </summary>
    public class CommissionId : TinyType<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionId"/> class.
        /// </summary>
        /// <param name="value">The unique Commission ID.</param>
        public CommissionId(int value)
            : base(value)
        {
        }
    }
}
