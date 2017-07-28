using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    public interface IStaticFileOptionsConfiguration
    {
        void Configure(StaticFileOptions options);
    }
}
