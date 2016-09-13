using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Web.OData;

namespace Cofoundry.Web.WebApi
{
    /// <summary>
    /// This class implements a JsonContractResolver to provide support for deserialization of the Delta<T> type
    /// using Json.NET. 
    /// </summary>
    /// <remarks>
    /// See http://stackoverflow.com/a/26390875/716689 - also parts taken from https://aspnet.codeplex.com/SourceControl/latest#Samples/WebApi/DeltaJsonDeserialization/ReadMe.txt
    ///
    /// The contract created for Delta<T> will deserialize properties using the types and property names of the 
    /// underlying type. The JsonProperty instances are copied from the underlying type's JsonContract and 
    /// customized to work with a dynamic object. In particular, a custom IValueProvider is used to get and set 
    /// values using the contract of DynamicObject, which Delta<T> inherits from.
    /// </remarks>
    public class DeltaContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            // This class special cases the JsonContract for just the Delta<T> class. All other types should function
            // as usual.
            if (objectType.IsGenericType &&
                objectType.GetGenericTypeDefinition() == typeof(Delta<>) &&
                objectType.GetGenericArguments().Length == 1)
            {
                var contract = CreateDynamicContract(objectType);
                contract.Properties.Clear();

                var underlyingContract = CreateObjectContract(objectType.GetGenericArguments()[0]);
                var underlyingProperties =
                    underlyingContract.CreatedType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in underlyingContract.Properties)
                {
                    property.DeclaringType = objectType;
                    property.ValueProvider = new DynamicObjectValueProvider()
                    {
                        PropertyName = this.ResolveName(underlyingProperties, property.PropertyName),
                    };

                    contract.Properties.Add(property);
                }

                return contract;
            }

            return base.CreateContract(objectType);
        }

        private string ResolveName(PropertyInfo[] properties, string propertyName)
        {
            var prop = properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (prop != null)
            {
                return prop.Name;
            }

            return propertyName;
        }

        private class DynamicObjectValueProvider : IValueProvider
        {
            public string PropertyName { get; set; }

            public object GetValue(object target)
            {
                DynamicObject d = (DynamicObject)target;

                object result;
                var binder = CreateGetMemberBinder(target.GetType(), PropertyName);
                d.TryGetMember(binder, out result);
                return result;
            }

            public void SetValue(object target, object value)
            {
                DynamicObject d = (DynamicObject)target;

                var binder = CreateSetMemberBinder(target.GetType(), PropertyName);
                d.TrySetMember(binder, value);
            }
        }

        private static GetMemberBinder CreateGetMemberBinder(Type type, string memberName)
        {
            return (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
                Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None,
                memberName,
                type,
                new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] 
                {
                });
        }

        private static SetMemberBinder CreateSetMemberBinder(Type type, string memberName)
        {
            return (SetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.SetMember(
                Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags.None,
                memberName,
                type,
                new Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] 
                {
                    Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags.None, null),
                });
        }
    }
}
