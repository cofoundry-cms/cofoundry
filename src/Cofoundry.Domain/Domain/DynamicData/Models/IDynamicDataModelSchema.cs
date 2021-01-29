using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Abstraction of a model that contains data model schema
    /// information e.g. schema information about a block type 
    /// or custom entity data model.  
    /// </summary>
    public interface IDynamicDataModelSchema
    {
        string DataTemplateName { get; set; }

        /// <summary>
        /// Data model property meta data, including UI display details
        /// and validation attributes. This is typically used for dynamically generating 
        /// parts of the admin UI.
        /// </summary>
        ICollection<DynamicDataModelSchemaProperty> DataModelProperties { get; set; }

        public DynamicDataModelDefaultValue DefaultValue { get; set; }
    }
}
