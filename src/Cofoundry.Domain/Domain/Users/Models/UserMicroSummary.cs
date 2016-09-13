using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class UserMicroSummary
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserAreaMicroSummary UserArea { get; set; }

        public string GetFullName()
        {
            return (FirstName + " " + LastName).Trim();
        }
    }
}
