using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BlazorInputFile;

namespace Mandarin.Models.Contact
{
    /// <summary>
    /// Represents the backing model for the Contact Us form.
    /// </summary>
    public class ContactDetailsModel : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the submitting user's email address for adding as a reply target.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the submitting user's name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the submitting user's reason for contacting.
        /// </summary>
        [Required]
        public ContactReasonType Reason { get; set; }

        /// <summary>
        /// Gets or sets the custom reason for contacting.
        /// </summary>
        [MaxLength(100, ErrorMessage = "The Other Reason field must not be longer than 100 characters.")]
        public string AdditionalReason { get; set; }

        /// <summary>
        /// Gets or sets the submitting user's contact message.
        /// </summary>
        [MaxLength(2500, ErrorMessage = "The Comment field must not be longer than 2500 characters.")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the list of items to be attached to the contact email form.
        /// </summary>
        public List<IFileListEntry> Attachments { get; set; } = new List<IFileListEntry>();

        /// <inheritdoc />
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.Reason == ContactReasonType.NotSelected)
            {
                yield return new ValidationResult("The Reason field must not be left unselected.",
                                                  new[] { nameof(this.Reason) });
            }

            if (this.Reason == ContactReasonType.Other && string.IsNullOrWhiteSpace(this.AdditionalReason))
            {
                yield return new ValidationResult("Please add your reason for contacting us.",
                                                  new[] { nameof(this.AdditionalReason) });
            }

            if (this.Attachments.Sum(x => x.Size) > 10 * 1024 * 1024)
            {
                yield return new ValidationResult("Maximum size of all attachments is 10MB.",
                                                  new[] { nameof(this.Attachments) });
            }
        }
    }
}
