using Newtonsoft.Json.Converters;

namespace Mandarin.Converters
{
    /// <summary>
    /// JsonConverter to format DateTime as "dd/MM/yyyy".
    /// </summary>
    public class IsoDateConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsoDateConverter"/> class.
        /// </summary>
        public IsoDateConverter()
        {
            this.DateTimeFormat = "dd/MM/yyyy";
        }
    }
}
