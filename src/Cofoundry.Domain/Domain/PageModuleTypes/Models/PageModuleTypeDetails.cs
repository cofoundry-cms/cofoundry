using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageModuleTypeDetails : PageModuleTypeSummary, IDynamicDataModelSchema
    {
        public string DataTemplateName { get; set; }

        public DynamicDataModelSchemaProperty[] DataModelProperties { get; set; }
    }
}
