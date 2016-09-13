using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.ModelBinding;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// A custom model binder for commands like AddPageVersionModuleCommand which is required to
    /// resolve the child PageModuleDataModel
    /// </summary>
    public class PageVersionModuleDataModelCommandModelBinder : IModelBinder
    {
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var jsonString = actionContext.Request.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(jsonString);
            var pageModuleTypeIdProperty = json.GetValue("PageModuleTypeId", StringComparison.OrdinalIgnoreCase);

            JsonConverter dataModelConverter;
            if (pageModuleTypeIdProperty == null)
            {
                dataModelConverter = new NullPageModuleDataModelJsonConverter();
            }
            else
            {
                dataModelConverter = GetModuleDataTypeConverter(pageModuleTypeIdProperty.Value<int>());
            }
            
            var result = JsonConvert.DeserializeObject(jsonString, bindingContext.ModelType, dataModelConverter);
            bindingContext.Model = result;

            return true;
        }

        private JsonConverter GetModuleDataTypeConverter(int pageModuleTypeId)
        {
            var moduleDataModelTypeFactory = Resolve<IModuleDataModelTypeFactory>();
            var dataModelType = moduleDataModelTypeFactory.CreateByPageModuleTypeId(pageModuleTypeId);
            var converterType = typeof(PageModuleDataModelJsonConverter<>).MakeGenericType(dataModelType);

            return (JsonConverter)Activator.CreateInstance(converterType);
        }

        private T Resolve<T>()
        {
            // Use DependencyResolver because IModelBinder are singletons and don't allow injection in the request lifecycle
            return IckyDependencyResolution.ResolveInWebApiContext<T>();
        }
    }
}