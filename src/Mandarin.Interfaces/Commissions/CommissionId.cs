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
        private CommissionId(int value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionId"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique Commission Id.</param>
        /// <returns>A newly created <see cref="CommissionId"/> or null/empty.</returns>
        public static CommissionId Of(int? value) => value.HasValue ? new CommissionId(value.Value) : null;
    }
}
