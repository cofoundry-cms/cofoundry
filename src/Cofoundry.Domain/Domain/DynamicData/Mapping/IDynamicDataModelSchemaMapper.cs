using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public interface IDynamicDataModelSchemaMapper
    {
        void Map(IDynamicDataModelSchema details, Type modelType);
    }
}
