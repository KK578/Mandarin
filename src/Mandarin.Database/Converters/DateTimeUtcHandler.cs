using System;
using System.Data;
using Dapper;

namespace Mandarin.Database.Converters
{
    /// <summary>
    /// Implements a <see cref="SqlMapper.TypeHandler{T}"/> that forces <see cref="DateTime"/> as UTC.
    /// </summary>
    internal sealed class DateTimeUtcHandler : SqlMapper.TypeHandler<DateTime>
    {
        /// <inheritdoc/>
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = value;
        }

        /// <inheritdoc/>
        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        }
    }
}
