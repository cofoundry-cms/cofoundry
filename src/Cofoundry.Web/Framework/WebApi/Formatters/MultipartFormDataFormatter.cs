using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Globalization;
using System.Web.OData;
using Cofoundry.Domain;

namespace Cofoundry.Web.WebApi
{
    public class MultipartFormDataFormatter : MediaTypeFormatter
    {
        #region constructor

        private const string MULTIPART_MEDIA_TYPE = "multipart/form-data";
        private const string APPLICATION_MEDIA_TYPE = "application/octet-stream";

        public MultipartFormDataFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(MULTIPART_MEDIA_TYPE));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(APPLICATION_MEDIA_TYPE));
        }

        #endregion

        #region base class implementation

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return false;
        }

        public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var obj = Activator.CreateInstance(type);

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Delta<>) &&
                type.GetGenericArguments().Length == 1)
            {
                Delta delta = (Delta)obj;
                var typeToBind = type.GetGenericArguments()[0];

                await ReadFormData(typeToBind, content, (prop, value) =>
                {
                    delta.TrySetPropertyValue(prop.Name, value);
                });
            }
            else
            {
                await ReadFormData(type, content, (prop, value) =>
                {
                    prop.SetValue(obj, value);
                });
            }
            
            return obj;
        }

        #endregion

        #region helpers

        private async Task<List<Tuple<string, HttpContent>>> GetFormFields(HttpContent content)
        {
            var parts = (await content
                .ReadAsMultipartAsync())
                .Contents
                .Select(c => new Tuple<string, HttpContent>(c.Headers.ContentDisposition.Name.Replace("\"", ""), c))
                .ToList();

            return parts;
        }

        private HttpContent GetFormField(List<Tuple<string, HttpContent>> formFields, PropertyInfo property)
        {
            var field = formFields
                .Where(f => f.Item1.Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Item2)
                .FirstOrDefault();

            return field;
        }

        private async Task ReadFormData(Type type, HttpContent content, Action<PropertyInfo, object> propertySetter)
        {
            var formFields = await GetFormFields(content);

            foreach (var property in type.GetRuntimeProperties())
            {
                var formData = GetFormField(formFields, property);
                if (formData == null) continue;
                object value = null;

                if (typeof(IUploadedFile).IsAssignableFrom(property.PropertyType) && formData.Headers.ContentLength > 0)
                {
                    value = new WebApiUploadedFile(formData);
                }
                else if (formData.Headers.ContentLength > 0)
                {
                    value = await ExtractValueFromFormField(property, formData);
                }
                else if (type.IsValueType)
                {
                    value = Activator.CreateInstance(type);
                }

                propertySetter(property, value);
            }
        }

        private async Task<object> ExtractValueFromFormField(PropertyInfo property, HttpContent formData)
        {
            var rawValue = await formData.ReadAsStringAsync();
            var valueType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            var typeConverter = GetFromStringConverter(valueType);
            object value;

            if (typeConverter != null)
            {
                value = typeConverter.ConvertFromString(null, CultureInfo.CurrentCulture, rawValue);
            }
            else
            {
                // for complex types, expect json format
                value = JsonConvert.DeserializeObject(rawValue, valueType);
            }

            return value;
        }

        private TypeConverter GetFromStringConverter(Type type)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
            if (typeConverter != null && !typeConverter.CanConvertFrom(typeof(String)))
            {
                typeConverter = null;
            }
            return typeConverter;
        }

        #endregion
    }
}
