using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Interface to mark classes that bootstrap DI container registration.
    /// </summary>
    public interface IDependencyRegistration
    {
        void Register(IContainerRegister container);
    }
}
