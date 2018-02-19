using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Used to get the html content for the admin panel dashboard.
    /// </summary>
    public class DashboardContentProvider : IDashboardContentProvider
    {
        private readonly IResourceLocator _resourceLocator;
        private readonly IQueryExecutor _queryExecutor;
        
        public DashboardContentProvider(
            IResourceLocator resourceLocator,
            IQueryExecutor queryExecutor
            )
        {
            _resourceLocator = resourceLocator;
            _queryExecutor = queryExecutor;
        }

        /// <summary>
        /// Returns the html content to be injected into the page for the admin 
        /// panel dashboard.
        /// </summary>
        public async Task<IHtmlContent> GetAsync()
        {
            const string CUSTOM_DASHBOARD_CONTENT_PATH = "/Cofoundry/Admin/Dashboard/Dashboard.html";
            const string DEFAULT_CONTENT = "<h2>{0}</h2><p>Welcome to the administration panel.</p> ";
            
            var result = _resourceLocator.GetFile(CUSTOM_DASHBOARD_CONTENT_PATH);
            string html;

            if (!result.Exists)
            {
                var settings = await _queryExecutor.ExecuteAsync(new GetSettingsQuery<GeneralSiteSettings>());
                html = string.Format(DEFAULT_CONTENT, settings.ApplicationName);
            }
            else
            {
                using (var stream = result.CreateReadStream())
                using (var reader = new StreamReader(stream))
                {
                    html = await reader.ReadToEndAsync();
                }
            }

            return new HtmlString(html);
        }
    }
}
