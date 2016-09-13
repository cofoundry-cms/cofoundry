using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// User information relating to a login request
    /// </summary>
    public class UserLoginInfo
    {
        public int UserId { get; set; }
        public string UserAreaCode { get; set; }
        public bool RequirePasswordChange { get; set; }
    }
}
