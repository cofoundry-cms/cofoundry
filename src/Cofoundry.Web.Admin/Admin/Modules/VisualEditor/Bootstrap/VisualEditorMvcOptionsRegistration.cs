using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using Cofoundry.Web.Admin.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorMvcOptionsConfiguration : IMvcOptionsConfiguration
    {
        private readonly PagesSettings _pagesSettings;
        private readonly AdminSettings _adminSettings;

        public VisualEditorMvcOptionsConfiguration(
            PagesSettings pagesSettings,
            AdminSettings adminSettings
            )
        {
            _pagesSettings = pagesSettings;
            _adminSettings = adminSettings;
        }

        public void Configure(MvcOptions options)
        {
            if (_pagesSettings.Disabled || !_adminSettings.AutoInjectVisualEditor) return;

            options.Filters.Add(typeof(VisualEditorContentFilter));
        }
    }
}