using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cofoundry.Web
{
    /// <summary>
    /// Use this in your controllers to return a 404 result using the Cofoundry custom 404 page system. This 
    /// has the added benefit of checking for Rewrite Rules and automatically redirecting.
    /// </summary>
    public class NotFoundViewHelper : INotFoundViewHelper
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageViewModelBuilder _pageViewModelBuilder;

        #region constructor

        public NotFoundViewHelper(
            IQueryExecutor queryExecutor,
            IPageViewModelBuilder pageViewModelBuilder
            )
        {
            _queryExecutor = queryExecutor;
            _pageViewModelBuilder = pageViewModelBuilder;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Use this in your controllers to return a 404 result using the Cofoundry custom 404 page system. This 
        /// has the added benefit of checking for Rewrite Rules and automatically redirecting.
        /// </summary>
        public async Task<ActionResult> GetViewAsync(Controller controller)
        {
            var path = controller.Request.Path;

            var result = await GetRewriteResultAsync(path);
            if (result != null) return result;

            var vmParams = new NotFoundPageViewModelBuilderParameters(path);
            var vm = await _pageViewModelBuilder.BuildNotFoundPageViewModelAsync(vmParams);

            controller.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return controller.View("NotFound", vm);
        }

        #endregion

        #region helpers

        /// <summary>
        /// If a page isnt found, check to see if we have a redirection rule
        /// in place for the url.
        /// </summary>
        private async Task<ActionResult> GetRewriteResultAsync(string path)
        {
            var query = new GetRewriteRuleByPathQuery() { Path = path };
            var rewriteRule = await _queryExecutor.ExecuteAsync(query);

            if (rewriteRule != null)
            {
                string writeTo = rewriteRule.WriteTo;
                return new RedirectResult(rewriteRule.WriteTo, true);
            }

            return null;
        }

        #endregion
    }
}