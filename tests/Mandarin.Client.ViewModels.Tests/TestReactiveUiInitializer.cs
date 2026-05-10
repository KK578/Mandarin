using System.Runtime.CompilerServices;
using ReactiveUI.Builder;

namespace Mandarin.Client.ViewModels.Tests
{
    public static class TestReactiveUiInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            RxAppBuilder.CreateReactiveUIBuilder()
                        .WithCoreServices()
                        .BuildApp();
        }
    }
}
