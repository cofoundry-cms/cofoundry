using Cofoundry.Core.Json;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Extends the MvcOptions configuration to add Cofoundry
    /// specific settings.
    /// </summary>
    public class CofoundryMvcOptionsConfiguration : IMvcOptionsConfiguration
    {
        public void Configure(MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new JsonDeltaModelBinderProvider());
        }
    }
}
