using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Registration
{
    /// <summary>
    /// Registers the default locations for page block type view files
    /// </summary>
    public class DefaultPageBlockTypeViewLocationRegistration : IPageBlockTypeViewLocationRegistration
    {
        public IEnumerable<string> GetPathPrefixes()
        {
            yield return "/PageBlockTypes";
            yield return "/Cofoundry/PageBlockTypes";
            yield return "/Views/PageBlockTypes";
        }
    }
}