using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public interface IUpdateAuditable : ICreateAuditable
    {
        User Updater { get; set; }
        DateTime UpdateDate { get; set; }
        int UpdaterId { get; set; }
    }
}
