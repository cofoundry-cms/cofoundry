using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using System.Net;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper designed to return ActionResults pointing to
    /// the 404/NotFound page
    /// </summary>
    public class NotFoundViewHelper : INotFoundViewHelper
    {
        private readonly IQueryExecutor _queryExecutor;

        #region constructor

        public NotFoundViewHelper(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region public methods

        public ActionResult GetView()
        {
            var path = HttpContext.Current.Request.Path;

            var result = GetRewriteResult(path);
            if (result != null) return result;

            var vm = new NotFoundPageViewModel
            {
                PageTitle = "Page not found",
                MetaDescription = "Sorry, that page could not be found"
            };

            HttpContext.Current.Response.StatusCode = 404;

            return new ViewResult()
            {
                ViewName = HttpStatusCode.NotFound.ToString(),
                ViewData = new ViewDataDictionary(vm)
            };
        }

        #endregion

        #region helpers

        /// <summary>
        /// If a page isnt found, check to see if we have a redirection rule
        /// in place for the url.
        /// </summary>
        private ActionResult GetRewriteResult(string path)
        {
            var query = new GetRewriteRuleByPathQuery() { Path = path };
            var rewriteRule = _queryExecutor.Execute(query);

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