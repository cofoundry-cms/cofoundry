using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    public class ExecuteDbServerScriptCommand
    {
        public string Script { get; set; }
        public DbConnectionInfo ConnectionInfo { get; set; }
    }
}
