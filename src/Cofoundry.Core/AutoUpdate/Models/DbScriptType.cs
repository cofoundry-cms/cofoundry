using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Prepresents the various types of scripts that can be run. The
    /// numerical values indicate the order in which they should be run.
    /// </summary>
    public enum DbScriptType
    {
        Schema = 10,
        Functions = 20,
        Views = 30,
        Triggers = 40,
        StoredProcedures = 50,
        Finalize = 100
    }
}
