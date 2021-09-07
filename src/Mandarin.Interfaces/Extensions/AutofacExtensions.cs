using Autofac;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Mandarin.Extensions
{
    /// <summary>
    /// Represents helpful extensions on Autofac types.
    /// </summary>
    public static class AutofacExtensions
    {
        /// <summary>
        /// Registers the type <typeparamref name="T"/> as an available configuration type.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <param name="section">The section name to be bound to.</param>
        /// <typeparam name="T">The type of the configuration to be configured.</typeparam>
        public static void RegisterConfiguration<T>(this ContainerBuilder builder, string section)
            where T : class
        {
            builder.Register(c => c.Resolve<IConfiguration>().GetSection(section).Get<T>()).SingleInstance();
            builder.Register(c => Options.Create(c.Resolve<T>())).SingleInstance();
        }

        /// <summary>
        /// Registers the type <typeparamref name="T"/> as an AutoMapper <see cref="Profile"/>.
        /// </summary>
        /// <param name="builder">Container builder.</param>
        /// <typeparam name="T">The type of the AutoMapper <see cref="Profile"/> to be configured.</typeparam>
        public static void RegisterProfile<T>(this ContainerBuilder builder)
            where T : Profile
        {
            builder.RegisterType<T>().As<Profile>();
        }
    }
}
