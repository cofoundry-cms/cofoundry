using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class UserArea
    {
        public string UserAreaCode { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
