using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.AutoUpdate
{
    /// <summary>
    /// Simple transaction abstraction
    /// framework
    /// </summary>
    public interface IDatabaseTransaction : IDisposable
    {
        void Commit();
    }
}
