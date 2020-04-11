using System;
using System.Threading.Tasks;
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
