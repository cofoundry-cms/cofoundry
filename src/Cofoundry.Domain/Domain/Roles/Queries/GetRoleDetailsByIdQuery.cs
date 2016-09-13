using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class GetRoleDetailsByIdQuery : IQuery<RoleDetails>
    {
        public GetRoleDetailsByIdQuery()
        {
        }

        public GetRoleDetailsByIdQuery(int? id)
        {
            RoleId = id;
        }
        public int? RoleId { get; set; }
    }
}
