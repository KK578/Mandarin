using Microsoft.AspNetCore.Components;

namespace Mandarin.Client.ViewModels.Tests.Helpers
{
    public sealed class MockNavigationManager : NavigationManager
    {
        public MockNavigationManager()
        {
            this.Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            this.Uri = this.ToAbsoluteUri(uri).ToString();
        }
    }
}
