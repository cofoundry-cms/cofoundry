using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IDynamicDataModelSchema
    {
        string DataTemplateName { get; set; }

        DynamicDataModelSchemaProperty[] DataModelProperties { get; set; }
    }
}
