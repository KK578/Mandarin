using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BlazorInputFile;

namespace Mandarin.Models.Contact
{
    public class ContactDetailsModel : IValidatableObject
    {
        [EmailAddress] public string Email { get; set; }

        [Required] public string Name { get; set; }

        [Required] public ContactReasonType Reason { get; set; }

        [MaxLength(100, ErrorMessage = "The Other Reason field must not be longer than 100 characters.")]
        public string AdditionalReason { get; set; }

        [MaxLength(2500, ErrorMessage = "The Comment field must not be longer than 2500 characters.")]
        public string Comment { get; set; }

        public List<IFileListEntry> Attachments { get; set; } = new List<IFileListEntry>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Reason == ContactReasonType.NotSelected)
            {
                yield return new ValidationResult("The Reason field must not be left unselected.", new[] { nameof(this.Reason) });
            }

            if (this.Reason == ContactReasonType.Other && string.IsNullOrWhiteSpace(this.AdditionalReason))
            {
                yield return new ValidationResult("Please add your reason for contacting us.", new[] { nameof(this.AdditionalReason) });
            }

            if (this.Attachments.Sum(x => x.Size) > 10 * 1024 * 1024)
            {
                yield return new ValidationResult("Maximum size of all attachments is 10MB.", new[] { nameof(this.Attachments) });
            }
        }
    }
}
