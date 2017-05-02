using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper to make it easier to return 404 (NotFound) responses using the
    /// built in 404 pages and rewrwite rule checking. 
    /// 
    /// See https://github.com/cofoundry-cms/cofoundry/wiki/Custom-Error-Pages for more information.
    /// </summary>
    public interface INotFoundViewHelper
    {
        /// <summary>
        /// Use this in your controllers to return a 404 result using the Cofoundry custom 404 page system. This 
        /// has the added benefit of checking for Rewrite Rules and automatically redirecting.
        /// </summary>
        Task<ActionResult> GetViewAsync(Controller controller);
    }
}