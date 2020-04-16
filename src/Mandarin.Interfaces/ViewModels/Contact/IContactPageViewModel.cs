using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorInputFile;
using Mandarin.Models.Contact;

namespace Mandarin.ViewModels.Contact
{
    public interface IContactPageViewModel
    {
        bool EnableAttachmentsUpload { get; }
        ContactDetailsModel Model { get; }

        bool LastSubmitSuccessful { get; }
        Exception SubmitException { get; }

        void OnFileChange(IEnumerable<IFileListEntry> files);
        Task SubmitAsync();
    }
}
