using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using Mandarin.Configuration;
using Mandarin.Models.Contact;
using Mandarin.Services;
using Microsoft.Extensions.Options;

namespace Mandarin.ViewModels.Contact
{
    /// <inheritdoc />
    internal sealed class ContactPageViewModel : IContactPageViewModel
    {
        private readonly IEmailService emailService;
        private readonly IOptions<MandarinConfiguration> configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactPageViewModel"/> class.
        /// </summary>
        /// <param name="emailService">The email service.</param>
        /// <param name="configuration">The application configuration.</param>
        public ContactPageViewModel(IEmailService emailService, IOptions<MandarinConfiguration> configuration)
        {
            this.emailService = emailService;
            this.configuration = configuration;
            this.Model = new ContactDetailsModel();
        }

        /// <inheritdoc/>
        public ContactDetailsModel Model { get; }

        /// <inheritdoc/>
        public bool EnableAttachmentsUpload => this.configuration.Value.EnableAttachments;

        /// <inheritdoc/>
        public bool LastSubmitSuccessful { get; private set; }

        /// <inheritdoc/>
        public Exception SubmitException { get; private set; }

        /// <inheritdoc/>
        public void UpdateAttachments(IEnumerable<IFileListEntry> files)
        {
            this.Model.Attachments = files.ToList();
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync()
        {
            try
            {
                var email = await this.emailService.BuildEmailAsync(this.Model);
                var response = await this.emailService.SendEmailAsync(email);
                if (response.IsSuccess)
                {
                    this.LastSubmitSuccessful = true;
                    this.SubmitException = null;
                }
                else
                {
                    this.LastSubmitSuccessful = false;
                    this.SubmitException = new Exception("Something went wrong.");
                }
            }
            catch (Exception ex)
            {
                this.LastSubmitSuccessful = false;
                this.SubmitException = ex;
            }
        }
    }
}
