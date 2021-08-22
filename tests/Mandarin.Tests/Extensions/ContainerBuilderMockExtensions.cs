using Autofac;
using Moq;

namespace Mandarin.Tests.Extensions
{
    public static class ContainerBuilderMockExtensions
    {
        public static void RegisterMock<T>(this ContainerBuilder builder)
            where T : class
        {
            builder.RegisterInstance(new Mock<T>()).AsSelf();
            builder.Register(c => c.ResolveMock<T>().Object);
        }

        public static Mock<T> ResolveMock<T>(this IComponentContext context)
            where T : class
        {
            return context.Resolve<Mock<T>>();
        }
    }
}
