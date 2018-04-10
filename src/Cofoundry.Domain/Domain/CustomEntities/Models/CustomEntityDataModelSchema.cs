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
    /// used for expressing dynamic data parts of Cofoundry (e.g. custom entities
    /// and page block data models) in dynamically generated parts of the UI
    /// </summary>
    public class CustomEntityDataModelSchema : IDynamicDataModelSchema
    {
        public string CustomEntityDefinitionCode { get; set; }

        public string DataTemplateName { get; set; }

        public ICollection<DynamicDataModelSchemaProperty> DataModelProperties { get; set; }
    }
}
