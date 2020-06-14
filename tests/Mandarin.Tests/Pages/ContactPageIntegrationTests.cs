using System;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Mandarin.Models.Contact;
using Mandarin.Tests.Factory;
using Mandarin.ViewModels.Contact;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace Mandarin.Tests.Pages
{
    [TestFixture]
    public class ContactPageIntegrationTests
    {
        private readonly WebApplicationFactory<MandarinStartup> factory;
        private HttpClient client;
        private ContactDetailsModel model;
        private Mock<IContactPageViewModel> viewModel;

        public ContactPageIntegrationTests()
        {
            this.factory = MandarinApplicationFactory.Create();
        }

        [SetUp]
        public void SetUp()
        {
            this.model = new ContactDetailsModel();
            this.viewModel = new Mock<IContactPageViewModel>();
            this.viewModel.SetupGet(x => x.Model).Returns(this.model);
            this.client = this.factory.WithWebHostBuilder(b => b.ConfigureServices(RegisterViewModel)).CreateClient();

            void RegisterViewModel(IServiceCollection services)
            {
                services.AddSingleton(this.viewModel.Object);
            }
        }

        [Test]
        [TestCase(ContactReasonType.NotSelected, 2)]
        [TestCase(ContactReasonType.Other, 3)]
        public async Task GivenReasonIsSetToOther_ThenAdditionalInputIsRendered(
            ContactReasonType reason,
            int expectedInputs)
        {
            this.GivenContactPageViewModelReason(reason);
            var contactPage = await this.WhenGetContactPage();
            Assert.That(contactPage.DocumentElement.QuerySelectorAll("input"), Has.Length.EqualTo(expectedInputs));
        }

        [Test]
        public async Task GivenEnableAttachmentsIsTrue_ThenAdditionalInputIsRendered()
        {
            this.GivenAttachmentUploadsAreEnabled();
            var contactPage = await this.WhenGetContactPage();
            Assert.That(contactPage.DocumentElement.QuerySelectorAll("input"), Has.Length.EqualTo(3));
        }

        [Test]
        public async Task GivenSubmissionIsComplete_ThenFormIsNotRendered()
        {
            this.GivenContactPageViewModelSubmissionComplete();
            var contactPage = await this.WhenGetContactPage();
            Assert.That(contactPage.DocumentElement.QuerySelectorAll("input"), Has.Length.Zero);
            Assert.That(contactPage.DocumentElement.TextContent, Contains.Substring("Thank you for your message!"));
        }

        [Test]
        public async Task GivenSubmissionFailedWithException_ThenFormShouldShowAnErrorMessage()
        {
            var exception = new Exception("Failed to send.");
            this.GivenContactPageViewModelSubmissionError(exception);
            var contactPage = await this.WhenGetContactPage();
            Assert.That(contactPage.DocumentElement.TextContent,
                        Contains.Substring("Sorry something went wrong... Try again in a moment."));
        }

        private void GivenContactPageViewModelReason(ContactReasonType reason)
        {
            this.model.Reason = reason;
        }

        private void GivenContactPageViewModelSubmissionError(Exception exception)
        {
            this.viewModel.SetupGet(x => x.SubmitException).Returns(exception);
        }

        private void GivenContactPageViewModelSubmissionComplete()
        {
            this.viewModel.SetupGet(x => x.LastSubmitSuccessful).Returns(true);
        }

        private void GivenAttachmentUploadsAreEnabled()
        {
            this.viewModel.SetupGet(x => x.EnableAttachmentsUpload).Returns(true);
        }

        private async Task<IDocument> WhenGetContactPage()
        {
            var response = await this.client.GetAsync("/contact");
            var content = await response.Content.ReadAsStringAsync();
            var document = await BrowsingContext.New().OpenAsync(req => req.Content(content));
            return document;
        }
    }
}
