using System.ComponentModel;

namespace Mandarin.Models.Contact
{
    /// <summary>
    /// Represents the reason for the "Contact Us" form on the Contact page.
    /// </summary>
    public enum ContactReasonType
    {
        /// <summary>
        /// No selection made yet.
        /// </summary>
        [Description("Reason for contacting us...")]
        NotSelected,

        /// <summary>
        /// General purpose query.
        /// </summary>
        [Description("General Query")]
        General,

        /// <summary>
        /// Artist requesting to stock with The Little Mandarin.
        /// </summary>
        [Description("Stocking with us")]
        ApplyForStocking,

        /// <summary>
        /// Contact request for The Mini Mandarin.
        /// </summary>
        [Description("Query for The Mini Mandarin")]
        MiniMandarin,

        /// <summary>
        /// Custom reason for contacting.
        /// </summary>
        [Description("Other (please specify)")]
        Other,
    }
}
