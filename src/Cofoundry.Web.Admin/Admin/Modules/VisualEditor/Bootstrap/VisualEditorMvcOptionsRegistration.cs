using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorMvcOptionsConfiguration : IMvcOptionsConfiguration
    {
        public void Configure(MvcOptions options)
        {
            options.Filters.Add(typeof(VisualEditorContentFilter));
        }
    }
}