using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public class EnumBinder<T> : IModelBinder where T : struct
    {
        private T? DefaultValue { get; set; }
        public EnumBinder(T? defaultValue)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            DefaultValue = defaultValue;
        }

        #region IModelBinder Members
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            return (value == null || string.IsNullOrEmpty(value.AttemptedValue))
                ? DefaultValue
                : GetEnumValue(DefaultValue, value.AttemptedValue);
        }

        #endregion

        public static U? GetEnumValue<U>(U? defaultValue, string value) where U : struct
        {
            U? enumType = defaultValue;

            if ((!String.IsNullOrEmpty(value)) && (Contains(typeof(T), value)))
                enumType = (U)Enum.Parse(typeof(U), value, true);

            return enumType;
        }

        public static bool Contains(Type enumType, string value)
        {
            return Enum.GetNames(enumType).Contains(value, StringComparer.OrdinalIgnoreCase);
        }
    }
}