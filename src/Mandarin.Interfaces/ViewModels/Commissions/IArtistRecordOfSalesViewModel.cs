using System.Threading.Tasks;
using Mandarin.Models.Commissions;

namespace Mandarin.ViewModels.Commissions
{
    /// <summary>
    /// Represents the component content for an artist's own record of sales (commission breakdown).
    /// </summary>
    public interface IArtistRecordOfSalesViewModel : IViewModel
    {
        /// <summary>
        /// Gets the summary of the artist's commissions.
        /// </summary>
        ArtistSales Commission { get; }

        /// <summary>
        /// Gets a value indicating whether an Email being sent is in progress.
        /// </summary>
        bool SendInProgress { get; }

        /// <summary>
        /// Gets a value indicating whether the last Email send operation was successful.
        /// </summary>
        bool SendSuccessful { get; }

        /// <summary>
        /// Gets the last Email send operation's status message.
        /// </summary>
        string StatusMessage { get; }

        /// <summary>
        /// Gets or sets the email address to send the artist's commission to.
        /// </summary>
        string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the customised message to include in the Record of Sales email.
        /// </summary>
        string CustomMessage { get; set; }

        /// <summary>
        /// Forces the state of the <see cref="SendSuccessful"/> flag to be toggled.
        /// This is used to allow the Record of Sales to be locked and not accept inputs.
        /// </summary>
        void ToggleSentFlag();

        /// <summary>
        /// Attempts to create and send the email to the Artist.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendEmailAsync();
    }
}
