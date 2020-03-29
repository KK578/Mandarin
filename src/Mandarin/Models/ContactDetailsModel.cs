using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mandarin.Models
{
    public class ContactDetailsModel : IValidatableObject
    {
        [EmailAddress] public string Email { get; set; }

        [Required] public string Name { get; set; }

        [Required] public ContactReasonType Reason { get; set; }

        [MaxLength(100, ErrorMessage = "Maximum 100 characters.")]
        public string AdditionalReason { get; set; }

        [MaxLength(2500, ErrorMessage = "Maximum 2500 characters.")]
        public string Comment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Reason == ContactReasonType.NotSelected)
                yield return new ValidationResult("Please select a contact reason.", new[] { nameof(this.Reason) });

            if (this.Reason == ContactReasonType.Other && string.IsNullOrWhiteSpace(this.AdditionalReason))
                yield return new ValidationResult("Please add your reason for contacting us.", new[] { nameof(this.AdditionalReason) });
        }
    }
}
