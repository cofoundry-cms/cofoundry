using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Retreives all page routes for custom entity details pages for a 
    /// specific custom entity definition. Typically there would only
    /// be a single page for a custom entity (e.g. blog post details)
    /// but it is possible to have multiple.
    /// </summary>
    public class GetPageRoutesByCustomEntityDefinitionCodeQuery : IQuery<ICollection<PageRoute>>
    {
        public GetPageRoutesByCustomEntityDefinitionCodeQuery() { }

        public GetPageRoutesByCustomEntityDefinitionCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        public string CustomEntityDefinitionCode { get; set; }
    }
}
