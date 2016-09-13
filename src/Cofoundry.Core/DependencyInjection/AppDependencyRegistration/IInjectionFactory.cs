using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Simple interface to be used for constructing types returned
    /// from the DI container.
    /// </summary>
    /// <typeparam name="T">Type to construct</typeparam>
    public interface IInjectionFactory<T>
    {
        T Create();
    }
}
