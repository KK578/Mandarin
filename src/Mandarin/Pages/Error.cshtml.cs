using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mandarin.Pages
{
    /// <summary>
    /// Represents the backing model for the Error Page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    [SuppressMessage("StyleCop", "SA1649", Justification = "Model is code behind for the Razor page.")]
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// Gets the request id for the request in error.
        /// </summary>
        public string RequestId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the request id should be shown.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

        /// <summary>
        /// Updates the RequestId when the page is ready to be displayed.
        /// </summary>
        public void OnGet()
        {
            this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
        }
    }
}
