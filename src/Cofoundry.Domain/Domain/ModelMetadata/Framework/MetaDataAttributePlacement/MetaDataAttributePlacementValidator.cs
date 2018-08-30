using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to validate that data model attributes are placed on properties of the
    /// correct type e.g. ImageAttribute must be placed on either an int or nullable
    /// int. Validation is designed to occur during the IMetadataAttribute.Process 
    /// method. If an invalid property type is found an IncorrectMetaDataAttributePlacementException
    /// is thrown.
    /// </summary>
    public static class MetaDataAttributePlacementValidator
    {
        /// <summary>
        /// Used to validate that data model attributes are placed on properties of the
        /// correct type e.g. ImageAttribute must be placed on either an int or nullable
        /// int. Validation is designed to occur during the IMetadataAttribute.Process 
        /// method. If an invalid property type is found an IncorrectMetaDataAttributePlacementException
        /// is thrown.
        /// </summary>
        /// <param name="attribute">The attribute to validate.</param>
        /// <param name="context">
        /// The context passed into the IMetadataAttribute.Process method, which is where this validation should be run.
        /// </param>
        /// <param name="validParamTypes">
        /// The valid property types to check for. At leats one must be provided.
        /// </param>
        public static void ValidatePropertyType(
            Attribute attribute,
            DisplayMetadataProviderContext context,
            params Type[] validParamTypes
            )
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (validParamTypes == null) throw new ArgumentNullException(nameof(validParamTypes));
            if (!validParamTypes.Any()) throw new ArgumentEmptyException(nameof(validParamTypes));

            var propertyModelType = context.Key.ModelType;

            if (!validParamTypes.Contains(propertyModelType))
            {
                throw new IncorrectMetaDataAttributePlacementException(attribute, context, validParamTypes);
            }
        }

        /// <summary>
        /// Used to validate that data model attributes are placed on properties of the
        /// correct type. This method is specifically for checking collection types where
        /// the type to check for is wrapped in some sort of enumerable e.g. ImageCollectionAttribute 
        /// must be placed on a collection of integers which could be ICollection&lt;int&gt;, 
        /// IEnumerable&lt;int&gt; or int[]. Validation is designed to occur during the 
        /// IMetadataAttribute.Process method. If an invalid property type is found an 
        /// IncorrectCollectionMetaDataAttributePlacementException is thrown.
        /// </summary>
        /// <param name="attribute">The attribute to validate.</param>
        /// <param name="context">
        /// The context passed into the IMetadataAttribute.Process method, which is where this validation should be run.
        /// </param>
        /// <param name="validParamTypes">
        /// The valid property types to check for. At leats one must be provided.
        /// </param>
        public static void ValidateCollectionPropertyType(
            Attribute attribute,
            DisplayMetadataProviderContext context,
            params Type[] validParamTypes
            )
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (validParamTypes == null) throw new ArgumentNullException(nameof(validParamTypes));
            if (!validParamTypes.Any()) throw new ArgumentEmptyException(nameof(validParamTypes));

            var propertyModelType = context.Key.ModelType;

            if (!typeof(IEnumerable).IsAssignableFrom(propertyModelType))
            {
                throw new IncorrectCollectionMetaDataAttributePlacementException(attribute, context, validParamTypes);
            }

            Type singularType = null;
            if (propertyModelType.IsGenericType)
            {
                var genericParameters = propertyModelType.GetGenericArguments();

                if (genericParameters?.Length != 1)
                {
                    throw new IncorrectCollectionMetaDataAttributePlacementException(attribute, context, validParamTypes);
                }

                singularType = genericParameters.Single();
            }
            else if (propertyModelType.IsArray)
            {
                singularType = propertyModelType.GetElementType();
            }

            if (singularType == null)
            {
                throw new IncorrectCollectionMetaDataAttributePlacementException(attribute, context, validParamTypes);
            }

            if (!validParamTypes.Contains(singularType))
            {
                throw new IncorrectCollectionMetaDataAttributePlacementException(attribute, context, validParamTypes);
            }
        }
    }
}
