using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A custom model binder for commands like AddCustomEntityCommand which is required to
    /// resolve the child PageModuleDataModel
    /// </summary>
    public class CustomEntityDataModelCommandModelBinder : IModelBinder
    {
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var jsonString = actionContext.Request.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(jsonString);
            var customEntityDefinitionCodeProperty = json.GetValue("CustomEntityDefinitionCode", StringComparison.OrdinalIgnoreCase);

            JsonConverter dataModelConverter;
            if (customEntityDefinitionCodeProperty == null)
            {
                dataModelConverter = new NullCustomEntityDataModelJsonConverter();
            }
            else
            {
                dataModelConverter = GetModuleDataTypeConverter(customEntityDefinitionCodeProperty.Value<string>());
            }
            
            var result = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType, dataModelConverter);
            bindingContext.Model = result;

            return true;
        }

        private JsonConverter GetModuleDataTypeConverter(string customEntityDefinitionCode)
        {
            var definitions = Resolve<ICustomEntityDefinition[]>();
            var definition = definitions.FirstOrDefault(d => d.CustomEntityDefinitionCode == customEntityDefinitionCode);
            var dataModelType = definition.GetDataModelType();
            var converterType = typeof(CustomEntityVersionDataModelJsonConverter<>).MakeGenericType(dataModelType);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }

        private T Resolve<T>()
        {
            // Use DependencyResolver because IModelBinder are singletons and don't allow injection in the request lifecycle
            return IckyDependencyResolution.ResolveInWebApiContext<T>();
        }
    }
}