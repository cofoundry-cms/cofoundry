using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Cofoundry.Web.WebApi
{
    public interface IWebApiStartupConfiguration
    {
        void Configure(HttpConfiguration config);
    }
}