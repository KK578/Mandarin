using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;

namespace Mandarin.Client.Tests.Helpers
{
    public abstract class MandarinTestContext : TestContext
    {
        protected MandarinTestContext()
        {
            this.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();
        }
    }
}
