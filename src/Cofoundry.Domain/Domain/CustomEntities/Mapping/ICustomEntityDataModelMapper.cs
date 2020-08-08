using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public interface ICustomEntityDataModelMapper
    {
        ICustomEntityDataModel Map(string customEntityDefinitionCode, string serializedData);
    }
}
