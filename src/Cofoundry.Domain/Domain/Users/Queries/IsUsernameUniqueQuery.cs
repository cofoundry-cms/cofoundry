using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsUsernameUniqueQuery : IQuery<bool>
    {
        public int? UserId { get; set; }

        public string Username { get; set; }

        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }
    }
}
