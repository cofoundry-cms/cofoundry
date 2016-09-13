using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetUserMicroSummaryByEmailQuery : IQuery<UserMicroSummary>
    {
        public GetUserMicroSummaryByEmailQuery(string email, string userAreaCode)
        {
            Email = email;
            UserAreaCode = userAreaCode;
        }

        public string UserAreaCode { get; set; }

        public string Email { get; set; }
    }
}
