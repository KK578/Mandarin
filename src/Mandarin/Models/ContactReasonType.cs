using System.ComponentModel;

namespace Mandarin.Models
{
    public enum ContactReasonType
    {
        [Description("Reason for contacting us...")] NotSelected,
        [Description("General Query")] General,
        [Description("Stocking with us")] ApplyForStocking,
        [Description("Query for The Mini Mandarin")] MiniMandarin,
        [Description("Other (please specify)")] Other,
    }
}
