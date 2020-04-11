using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Mandarin.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T value) where T : Enum
        {
            var type = typeof(T);
            var enumName = Enum.GetName(type, value);
            if (enumName == null)
            {
                throw new ArgumentException($"Could not find '{value}' in enum {type.Name}.");
            }

            var member = type.GetMember(enumName).FirstOrDefault();
            var attribute = member?.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
