using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Dependency resolution from a DI child container. This is the same as IResolutionContext
    /// but implements IDisposable so the child lifetime scope lifetime can be managed manually.
    /// </summary>
    public interface IChildResolutionContext : IResolutionContext, IDisposable
    {
    }
}
