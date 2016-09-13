using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchRolesQuery : SimplePageableQuery, IQuery<PagedQueryResult<RoleMicroSummary>>
    {
        public string Text { get; set; }

        public bool ExcludeAnonymous { get; set; }

        public string UserAreaCode { get; set; }
    }
}
