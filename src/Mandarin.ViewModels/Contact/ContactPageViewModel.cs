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
    internal sealed class ContactPageViewModel : IContactPageViewModel
    {
        private readonly IEmailService emailService;
        private readonly IOptions<MandarinConfiguration> configuration;

        public ContactDetailsModel Model { get; }

        public bool EnableAttachmentsUpload => this.configuration.Value.EnableAttachments;
        public bool LastSubmitSuccessful { get; private set; }
        public Exception SubmitException { get; private set; }

        public ContactPageViewModel(IEmailService emailService, IOptions<MandarinConfiguration> configuration)
        {
            this.emailService = emailService;
            this.configuration = configuration;
            this.Model = new ContactDetailsModel();
        }

        public void UpdateAttachments(IEnumerable<IFileListEntry> files)
        {
            this.Model.Attachments = files.ToList();
        }

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
            }
            catch (Exception ex)
            {
                this.LastSubmitSuccessful = false;
                this.SubmitException = ex;
            }
        }
    }
}
