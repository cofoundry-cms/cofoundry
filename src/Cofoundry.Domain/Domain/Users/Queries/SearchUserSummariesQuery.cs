using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchUserSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<UserSummary>>
    {
        [Required]
        public string UserAreaCode { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}
