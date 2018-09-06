using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data.SimpleDatabase
{
    /// <summary>
    /// Simple Db raw sql execution on the Cofoundry database which by
    /// default used the shared Cofoundry connection with a scoped lifetime. 
    /// This can be used in place of an EF DbContext to avoid a dependency on 
    /// any particular framework. Does not support connection resiliency 
    /// or retry logic.
    /// </summary>
    public interface ICofoundryDatabase : IDatabase
    {
    }
}
