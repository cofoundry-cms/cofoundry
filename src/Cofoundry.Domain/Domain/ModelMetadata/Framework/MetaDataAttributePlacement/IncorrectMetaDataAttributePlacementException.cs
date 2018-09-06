using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Thrown when a data model attribute (e.g. CustomEntityAttribute
    /// or ImageAttribute) is not placed on an property of the correct type.
    /// </summary>
    public class IncorrectMetaDataAttributePlacementException : Exception
    {
        /// <summary>
        /// Thrown when a data model attribute (e.g. CustomEntityAttribute
        /// or ImageAttribute) is not placed on an property of the correct type.
        /// </summary>
        public IncorrectMetaDataAttributePlacementException(
            Attribute attribute,
            DisplayMetadataProviderContext context,
            ICollection<Type> validParamTypes,
            string message
            )
            : base(message)
        {
            AttributeType = attribute?.GetType();
            ValidPropertyTypes = validParamTypes;
            ModelType = context?.Key.ContainerType;
            PropertyName = FormatPropertyName(context);
        }

        /// <summary>
        /// Thrown when a data model attribute (e.g. CustomEntityAttribute
        /// or ImageAttribute) is not placed on an property of the correct type.
        /// </summary>
        public IncorrectMetaDataAttributePlacementException(
            Attribute attribute,
            DisplayMetadataProviderContext context,
            ICollection<Type> validParamTypes
            )
            : this(attribute, context, validParamTypes, FormatMessage(attribute, context, validParamTypes))
        {
        }

        /// <summary>
        /// The type of the attribute that is placed incorrectly.
        /// </summary>
        public Type AttributeType { get; private set; }

        /// <summary>
        /// The property types which are valid types to be decorated. 
        /// E.g. one attribute might require a string property type, 
        /// whereas another property might require an int or nullable int
        /// type property.
        /// </summary>
        public virtual ICollection<Type> ValidPropertyTypes { get; private set; }

        /// <summary>
        /// The type of the model that contains the incorrect attribute 
        /// placement.
        /// </summary>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Formatted name of the offending property, using the 
        /// format 'ModelName.PropertyName'.
        /// </summary>
        public string PropertyName { get; private set; }

        private static string FormatMessage(
            Attribute attribute,
            DisplayMetadataProviderContext context,
            ICollection<Type> validParamTypes
            )
        {
            string typeNameText = FormatTypeNamesText(validParamTypes);
            string propertyName = FormatPropertyName(context);
            var attributeName = attribute.GetType().Name;

            return $"{attributeName} can only be placed on properties of {typeNameText} type. Property name is {propertyName} and the type is {context.Key.ModelType}.";
        }

        protected static string FormatPropertyName(DisplayMetadataProviderContext context)
        {
            return context.Key.ContainerType.Name + "." + context.Key.Name;
        }

        protected static string FormatTypeNamesText(ICollection<Type> validParamTypes)
        {
            var typeNames = validParamTypes.Select(t => t.ToString());
            string typeNameText;

            if (validParamTypes.Count <= 2)
            {
                typeNameText = string.Join(" or ", typeNames); ;
            }
            else
            {
                var firstTypeNames = typeNames.Take(validParamTypes.Count - 1);
                typeNameText = string.Join(", ", firstTypeNames) + " or " + typeNames.Last();
            }

            return typeNameText;
        }
    }
}
