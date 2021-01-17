using System.ComponentModel;

namespace Mandarin.Common
{
    /// <summary>
    /// Represents the status of a user or item.
    /// </summary>
    public enum StatusMode
    {
        /// <summary>
        /// The status of the item is unknown.
        /// </summary>
        [Description("Unknown")]
        Unknown = 0,

        /// <summary>
        /// The item is inactive.
        /// </summary>
        [Description("Inactive")]
        Inactive = 1,

        /// <summary>
        /// The item is active, but should not be publicly visible.
        /// </summary>
        [Description("Active (Hidden)")]
        ActiveHidden = 2,

        /// <summary>
        /// The item is active and publicly visible.
        /// </summary>
        [Description("Active")]
        Active = 4,
    }
}
