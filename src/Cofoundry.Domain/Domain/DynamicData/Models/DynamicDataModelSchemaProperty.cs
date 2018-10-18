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
    /// data models) in dynamically generated parts of the UI.
    /// </summary>
    public class DynamicDataModelSchemaProperty
    {
        /// <summary>
        /// The property name e.g. "ShortDescription".
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// An identifier that can be used as a UI hint to
        /// indicate which UI control or template to render
        /// for the property.
        /// </summary>
        public string DataTemplateName { get; set; }

        /// <summary>
        /// A description of the property that can be used to
        /// help a user when entering data.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A user-friendly display name, typically used when labelling
        /// a field e.g. "Short description".
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Indicates if the property should be validated
        /// as a required field.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Any additional attributes (key-values) that can be used by 
        /// the UI layer to render correctly. This could include validation
        /// properties, display hints, option data sources etc.
        /// </summary>
        public Dictionary<string, object> AdditionalAttributes { get; set; }
    }
}
