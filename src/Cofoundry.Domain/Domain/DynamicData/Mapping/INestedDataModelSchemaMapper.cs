using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cofoundry.Domain
{
    public interface INestedDataModelSchemaMapper
    {
        NestedDataModelSchema Map(Type modelType);
    }
}
