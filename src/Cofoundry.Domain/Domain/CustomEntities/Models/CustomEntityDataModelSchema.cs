using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Meta information about a data model, including UI display details
    /// and validation attributes for each public property. This is typically 
    /// used for expressing these entities in dynamically generated parts of 
    /// the UI e.g. edit forms and lists.
    /// </summary>
    public class CustomEntityDataModelSchema : IDynamicDataModelSchema
    {
        /// <summary>
        /// The six character definition code that represents the type of custom
        /// entity e.g. Blog Post, Project, Product. The definition code is defined
        /// in a class that inherits from ICustomEntityDefinition.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

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
