using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorMvcOptionsConfiguration : IMvcOptionsConfiguration
    {
        private readonly PagesSettings _pagesSettings;

        public VisualEditorMvcOptionsConfiguration(
            PagesSettings pagesSettings
            )
        {
            _pagesSettings = pagesSettings;
        }

        public void Configure(MvcOptions options)
        {
            if (_pagesSettings.Disabled) return;

            options.Filters.Add(typeof(VisualEditorContentFilter));
        }
    }
}