using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helpers to make it easier to work with the ModelMetaData class
    /// </summary>
    public static class ModelMetadataExtensions
    {
        /// <summary>
        /// Adds a value to the AdditionalValues collection if the specified condition is true.
        /// </summary>
        /// <param name="condition">Result of a condition which will indicate whether to add the value or not</param>
        /// <param name="property">The name of the property (key) to add to the collection.</param>
        /// <param name="value">The value to add to the collection.</param>
        /// <returns>ModelMetadata instance for method chaining</returns>
        public static DisplayMetadata AddAdditionalValueIf(this DisplayMetadata modelMetaData, bool condition, string property, object value)
        {
            if (condition)
            {
                modelMetaData.AdditionalValues.Add(property, value);
            }

            return modelMetaData;
        }

        /// <summary>
        /// Adds a value to the AdditionalValues collection if the value is not null
        /// </summary>
        /// <param name="property">The name of the property (key) to add to the collection.</param>
        /// <param name="value">The value to add to the collection.</param>
        /// <returns>ModelMetadata instance for method chaining</returns>
        public static DisplayMetadata AddAdditionalValueIfNotNull(this DisplayMetadata modelMetaData, string property, object value)
        {
            if (value != null)
            {
                modelMetaData.AdditionalValues.Add(property, value);
            }

            return modelMetaData;
        }

        /// <summary>
        /// Adds a value to the AdditionalValues collection if the value is not null or empty (default value)
        /// </summary>
        /// <param name="property">The name of the property (key) to add to the collection.</param>
        /// <param name="value">The value to add to the collection.</param>
        /// <returns>ModelMetadata instance for method chaining</returns>
        public static DisplayMetadata AddAdditionalValueIfNotEmpty<T>(this DisplayMetadata modelMetaData, string property, T value)
        {
            if (value != null && !EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                modelMetaData.AdditionalValues.Add(property, value);
            }

            return modelMetaData;
        }

        /// <summary>
        /// Adds a collection value to the AdditionalValues collection if the collection is not null or empty
        /// </summary>
        /// <param name="property">The name of the property (key) to add to the collection.</param>
        /// <param name="value">The collection value to add to the collection.</param>
        /// <returns>ModelMetadata instance for method chaining</returns>
        public static DisplayMetadata AddAdditionalValueIfNotEmpty<T>(this DisplayMetadata modelMetaData, string property, ICollection<T> value)
        {
            if (value != null && value.Any())
            {
                modelMetaData.AdditionalValues.Add(property, value);
            }

            return modelMetaData;
        }

        /// <summary>
        /// Adds a value to the AdditionalValues collection with a formatted validation message taken from a 
        /// ValidationAttribute. Intended to be used as a shortcut for adding validation messages associated with 
        /// ValidationAttributes in an implementation of IMetadataAttribute
        /// </summary>
        /// <param name="key">The name of the property (key) to add to the collection.</param>
        /// <param name="value">The value to add to the collection.</param>
        /// <param name="attribute">The attribute from which the validation message will be extracted.</param>
        /// <returns>ModelMetadata instance for method chaining</returns>
        public static DisplayMetadata AddAdditionalValueWithValidationMessage(this DisplayMetadata modelMetaData, string key, object value, ValidationAttribute attribute)
        {
            modelMetaData.AdditionalValues.Add(key, value);
            modelMetaData.AdditionalValues.Add(key + "ValMsg", attribute.FormatErrorMessage(modelMetaData.DisplayName()));

            return modelMetaData;
        }
    }
}
