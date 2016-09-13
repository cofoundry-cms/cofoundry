using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// A ContainerBuilder is an abstraction for DI container initialization. Use this class 
    /// to run all IContainerRegister implementation in the project and in all referenced modules
    /// at the initialization point of your application.
    /// </summary>
    public interface IContainerBuilder
    {
        /// <summary>
        /// Runs the container initialization process. This can only be run once and
        /// should be done at the initialization stage of your application.
        /// </summary>
        void Build();
    }
}
