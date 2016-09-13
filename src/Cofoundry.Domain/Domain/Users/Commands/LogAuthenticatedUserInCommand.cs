using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class LogAuthenticatedUserInCommand : ICommand
    {
        [PositiveInteger]
        public int UserId { get; set; }
    }
}
