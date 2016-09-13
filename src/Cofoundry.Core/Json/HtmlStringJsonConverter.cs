using Cofoundry.Core.AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Core.Json
{
    public class HtmlStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IHtmlString).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = reader.Value as string;

            if (objectType == typeof(ProjectableHtmlString))
            {
                return new ProjectableHtmlString(value);
            }

            if (objectType == typeof(HtmlString))
            {
                return new HtmlString(value);
            }

            return Activator.CreateInstance(objectType, value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, Convert.ToString(value));
        }
    }
}
