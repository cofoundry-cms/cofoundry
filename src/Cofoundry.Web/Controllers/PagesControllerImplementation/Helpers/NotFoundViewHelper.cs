using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using System.Net;
using System.Threading.Tasks;

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

        [Obsolete("This api has been removed in favor of an async version. Please use GetViewAsync instead.")]
        public ActionResult GetView()
        {
            throw new NotImplementedException("This api has been removed in favor of an async version. Please use GetViewAsync instead.");
        }

        /// <summary>
        /// Use this in your controllers to return a 404 result using the Cofoundry custom 404 page system. This 
        /// has the added benefit of checking for Rewrite Rules and automatically redirecting.
        /// </summary>
        public async Task<ActionResult> GetViewAsync()
        {
            var path = HttpContext.Current.Request.Path;

            var result = await GetRewriteResultAsync(path);
            if (result != null) return result;

            var vmParams = new NotFoundPageViewModelBuilderParameters(path);
            var vm = await _pageViewModelBuilder.BuildNotFoundPageViewModelAsync(vmParams);

            HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return new ViewResult()
            {
                ViewName = "NotFound",
                ViewData = new ViewDataDictionary(vm)
            };
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