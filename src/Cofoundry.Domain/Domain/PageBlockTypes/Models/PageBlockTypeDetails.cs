using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Page block types represent a type of content that can be inserted into a content 
    /// region of a page which could be simple content like 'RawHtml', 'Image' or 
    /// 'PlainText'. Custom and more complex block types can be defined by a 
    /// developer. Block types are typically created when the application
    /// starts up in the auto-update process.
    /// </para>
    /// <para>
    /// The PageBlockTypeDetails projection extends the PageBlockTypeSummary model and
    /// contains additional data model schema meta data.
    /// </para>
    /// </summary>
    public class PageBlockTypeDetails : PageBlockTypeSummary, IDynamicDataModelSchema
    {
        public string DataTemplateName { get; set; }

        /// <summary>
        /// The block type data model property meta data, including UI display details
        /// and validation attributes. This is typically used for dynamically generating 
        /// parts of the admin UI.
        /// </summary>
        public ICollection<DynamicDataModelSchemaProperty> DataModelProperties { get; set; }

        public DynamicDataModelDefaultValue DefaultValue { get; set; }
    }
}
