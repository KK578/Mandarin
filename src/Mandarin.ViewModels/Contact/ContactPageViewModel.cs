using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using Mandarin.Models.Contact;
using Mandarin.Services.Email;

namespace Mandarin.ViewModels.Contact
{
    internal sealed class ContactPageViewModel : IContactPageViewModel
    {
        private readonly IEmailService emailService;

        public ContactDetailsModel Model { get; }
        public bool LastSubmitSuccessful { get; private set; }
        public Exception SubmitException { get; private set; }

        public ContactPageViewModel(IEmailService emailService)
        {
            this.emailService = emailService;
            this.Model = new ContactDetailsModel();
        }

        public void OnFileChange(IEnumerable<IFileListEntry> files)
        {
            this.Model.Attachments = files.ToList();
        }

        public async Task SubmitAsync()
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
