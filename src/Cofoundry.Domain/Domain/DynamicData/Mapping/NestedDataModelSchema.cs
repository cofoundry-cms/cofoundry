using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Meta information about a data model, including UI display details
    /// and validation attributes for each public property. This is typically 
    /// used for expressing dynamic data parts of Cofoundry (e.g. custom entities
    /// and page block data models) in dynamically generated parts of the UI
    /// </summary>
    public class NestedDataModelSchema : IDynamicDataModelSchema
    {
        /// <summary>
        /// The name of the .NET data model type that the schema has been generated from.
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        /// The user-friendly display name, which is used to pick the model from a list
        /// in the admin panel.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Optional description of the data model, which is used to pick the model from a list
        /// in the admin panel.
        /// </summary>
        public string Description { get; set; }

        public string DataTemplateName { get; set; }

        /// <summary>
        /// Data model property meta data, including UI display details
        /// and validation attributes. This is typically used for dynamically generating 
        /// parts of the admin UI.
        /// </summary>
        public ICollection<DynamicDataModelSchemaProperty> DataModelProperties { get; set; }

        public DynamicDataModelDefaultValue DefaultValue { get; set; }
    }
}
