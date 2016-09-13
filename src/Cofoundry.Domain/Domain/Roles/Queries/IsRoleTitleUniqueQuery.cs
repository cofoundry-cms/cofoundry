using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsRoleTitleUniqueQuery : IQuery<bool>
    {
        public int? RoleId { get; set; }

        [Required]
        public string UserAreaCode { get; set; }

        public string Title { get; set; }
    }
}
