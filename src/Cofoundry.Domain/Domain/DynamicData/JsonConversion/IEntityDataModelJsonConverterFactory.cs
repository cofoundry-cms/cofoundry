using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public interface IEntityDataModelJsonConverterFactory
    {
        JsonConverter Create(Type dataModelType);
    }
}
