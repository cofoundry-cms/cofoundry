using Cofoundry.Core.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Model to be used in collections of different types of nested data models 
    /// that allows the type information to be stored alongside the model data.
    /// </summary>
    public class NestedDataModelMultiTypeItem
    {
        /// <summary>
        /// The type name of the concrete data model type. The model
        /// type must implement INestedDataModel.
        /// </summary>
        [Required]
        public string TypeName { get; set; }

        /// <summary>
        /// The model data to stored as unstructured data.
        /// </summary>
        [ValidateObject]
        public INestedDataModel Model { get; set; }
    }
}
