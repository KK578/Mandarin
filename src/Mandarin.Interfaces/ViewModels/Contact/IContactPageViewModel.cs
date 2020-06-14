using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorInputFile;
using Mandarin.Models.Contact;

namespace Mandarin.ViewModels.Contact
{
    /// <summary>
    /// Represents the page content for the Contact Page.
    /// </summary>
    public interface IContactPageViewModel
    {
        /// <summary>
        /// Gets the main underlying model for the Contact form.
        /// </summary>
        ContactDetailsModel Model { get; }

        /// <summary>
        /// Gets a value indicating whether attachments are allowed to be uploaded.
        /// </summary>
        bool EnableAttachmentsUpload { get; }

        /// <summary>
        /// Gets a value indicating whether the last form submission was successful.
        /// </summary>
        bool LastSubmitSuccessful { get; }

        /// <summary>
        /// Gets the last exception from the submission attempt if it failed.
        /// </summary>
        Exception SubmitException { get; }

        /// <summary>
        /// Updates the list of attachments to the provided file list.
        /// </summary>
        /// <param name="files">List of file entries to be added.</param>
        void UpdateAttachments(IEnumerable<IFileListEntry> files);

        /// <summary>
        /// Creates and sends the Contact email to The Little Mandarin.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SendEmailAsync();
    }
}
