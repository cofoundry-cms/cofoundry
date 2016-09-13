using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public interface ICreateAuditable
    {
        User Creator { get; set; }
        DateTime CreateDate { get; set; }
        int CreatorId { get; set; }
    }
}
