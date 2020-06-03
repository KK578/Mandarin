using Newtonsoft.Json.Converters;

namespace Mandarin.Converters
{
    public class IsoDateConverter : IsoDateTimeConverter
    {
        public IsoDateConverter()
        {
            this.DateTimeFormat = "dd/MM/yyyy";
        }
    }
}
