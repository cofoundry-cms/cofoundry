using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchPageTemplateFilesQuery : SimplePageableQuery, IQuery<PagedQueryResult<PageTemplateFile>>
    {
        public string Name { get; set; }

        /// <summary>
        /// Indictaes whether to include layout files that have already
        /// been registered in the result set. Default is false.
        /// </summary>
        public bool ExcludeRegistered { get; set; }
    }
}
