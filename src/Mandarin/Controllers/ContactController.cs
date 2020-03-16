using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mandarin.Controllers
{
    [Route("api/contact")]
    public sealed class ContactController : ControllerBase
    {
        private readonly IEmailService emailService;

        public ContactController(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        [HttpPost]
        public async Task<ActionResult> ContactUs(ContactDetailsModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var response = this.emailService.SendEmailAsync();
            return this.StatusCode(response.StatusCode);
        }
    }

    public class EmailResponse
    {
        public EmailResponse(int statusCode) => this.StatusCode = statusCode;

        public int StatusCode { get; }
    }

    public class ContactDetailsModel
    {
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Reason { get; set; }

        public string AdditionalReason { get; set; }

        [MaxLength(2500, ErrorMessage = "Maximum 2500 Characters.")]
        public string Comment { get; set; }
    }
}
