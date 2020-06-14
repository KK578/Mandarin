using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bashi.Tests.Framework.Data;
using BlazorInputFile;
using Mandarin.Models.Contact;
using Moq;
using NUnit.Framework;

namespace Mandarin.Interfaces.Tests.Models.Contact
{
    [TestFixture]
    public class ContactDetailsModelTests
    {
        [Test]
        public void OnValidate_NameMustBeSpecified()
        {
            var model = ContactDetailsModelTests.CreateValidModel();
            model.Name = null;
            this.AssertErrorMessages(model, "The Name field is required.");
            model.Name = TestData.NextString();
            this.AssertNoErrors(model);
        }

        [Test]
        public void OnValidate_EmailMustBeValid()
        {
            var model = ContactDetailsModelTests.CreateValidModel();
            model.Email = "InvalidEmail";
            this.AssertErrorMessages(model, "The Email field is not a valid e-mail address.");
            model.Email = "AValid@Email.com";
            this.AssertNoErrors(model);
        }

        [Test]
        public void OnValidate_CommentCannotBeLongerThan2500Characters()
        {
            var model = ContactDetailsModelTests.CreateValidModel();
            model.Comment = string.Join(string.Empty, Enumerable.Range(0, 1000).Select(x => "Hello"));
            this.AssertErrorMessages(model, "The Comment field must not be longer than 2500 characters.");
            model.Comment = "Hello World";
            this.AssertNoErrors(model);
        }

        [Test]
        public void OnValidate_ReasonMustNotBeNotSelected()
        {
            var model = ContactDetailsModelTests.CreateValidModel();
            model.Reason = ContactReasonType.NotSelected;
            this.AssertErrorMessages(model, "The Reason field must not be left unselected.");
            model.Reason = ContactReasonType.General;
            this.AssertNoErrors(model);
        }

        [Test]
        public void OnValidate_WhenReasonIsOther_AdditionalReasonMustBeSpecified()
        {
            var model = ContactDetailsModelTests.CreateValidModel();
            model.Reason = ContactReasonType.Other;
            this.AssertErrorMessages(model, "Please add your reason for contacting us.");
            model.AdditionalReason = "My particular reason.";
            this.AssertNoErrors(model);
        }

        [Test]
        public void OnValidate_AttachmentsCannotSumToLargerThan10Megabytes()
        {
            var model = ContactDetailsModelTests.CreateValidModel();
            model.Attachments = new List<IFileListEntry> { Mock.Of<IFileListEntry>(x => x.Size == 15_000_000) };
            this.AssertErrorMessages(model, "Maximum size of all attachments is 10MB.");
            model.Attachments = new List<IFileListEntry> { Mock.Of<IFileListEntry>(x => x.Size == 5_000_000) };
            this.AssertNoErrors(model);
        }

        private static ContactDetailsModel CreateValidModel()
        {
            return new ContactDetailsModel
            {
                Name = TestData.NextString(),
                Email = TestData.NextString() + "@googlemail.com",
                Reason = ContactReasonType.General,
                Attachments = new List<IFileListEntry>(),
                Comment = TestData.NextString(),
            };
        }

        private static List<ValidationResult> GetValidationErrors(ContactDetailsModel model)
        {
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }

        private void AssertErrorMessages(ContactDetailsModel model, params string[] expectedErrors)
        {
            var errors = ContactDetailsModelTests.GetValidationErrors(model);
            Assert.That(errors.Count, Is.EqualTo(expectedErrors.Length));
            for (var i = 0; i < expectedErrors.Length; i++)
            {
                Assert.That(errors[i].ErrorMessage, Is.EqualTo(expectedErrors[i]));
            }
        }

        private void AssertNoErrors(ContactDetailsModel model)
        {
            var errors = ContactDetailsModelTests.GetValidationErrors(model);
            Assert.That(errors.Count, Is.Zero);
        }
    }
}
