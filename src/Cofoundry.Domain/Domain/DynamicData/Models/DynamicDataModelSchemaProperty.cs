using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Represents a property of a data model, including UI display details
    /// and validation attributes. This is typically used for expressing
    /// dynamic data parts of Cofoundry (e.g. custom entities and page block 
    /// data models) in dynamically generated parts of the UI
    /// </summary>
    public class DynamicDataModelSchemaProperty
    {
        public string Name { get; set; }

        public string DataTemplateName { get; set; }

        public string Description { get; set; }

        public Dictionary<string, object> AdditionalAttributes { get; set; }

        public string DisplayName { get; set; }

        public bool IsRequired { get; set; }
    }
}
