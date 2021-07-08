using Blazorise;

namespace Mandarin.Client.Components.Inputs
{
    /// <summary>
    /// Represents a wrapper for a FontAwesome Icon.
    /// </summary>
    /// <param name="IconClass">The CSS class family that the FontAwesome Icon belongs to.</param>
    /// <param name="IconName">The CSS class that identifies the FontAwesome Icon.</param>
    /// <param name="IconStyle">The Icon style weight to be set.</param>
    public sealed record MandarinIcon(string IconClass, string IconName, IconStyle IconStyle)
    {
        /// <summary>
        /// Represents the Twitter FontAwesome Icon.
        /// </summary>
        public static readonly MandarinIcon Twitter = new("fab", "fa-twitter", IconStyle.Light);

        /// <summary>
        /// Represents the Facebook FontAwesome Icon.
        /// </summary>
        public static readonly MandarinIcon Facebook = new("fab", "fa-facebook", IconStyle.Light);

        /// <summary>
        /// Represents the Instagram FontAwesome Icon.
        /// </summary>
        public static readonly MandarinIcon Instagram = new("fab", "fa-instagram", IconStyle.Light);

        /// <summary>
        /// Represents the Tumblr FontAwesome Icon.
        /// </summary>
        public static readonly MandarinIcon Tumblr = new("fab", "fa-tumblr", IconStyle.Light);

        /// <summary>
        /// Represents the Url FontAwesome Icon.
        /// </summary>
        public static readonly MandarinIcon Url = new(string.Empty, "fa-globe-europe", IconStyle.Solid);
    }
}
