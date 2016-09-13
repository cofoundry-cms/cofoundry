using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Marker class to indicate Cofoundry settings category classes that are backed by the
    /// Settings db table. Clsses implementing this interface should also implement
    /// a query handler for a Get(ICofoundrySettings) query.
    /// </summary>
    public interface ICofoundrySettings
    {
    }
}
