using Cofoundry.Domain;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Web
{
    public interface IAuthCookieNamespaceProvider
    {
        string GetNamespace();
    }
}
