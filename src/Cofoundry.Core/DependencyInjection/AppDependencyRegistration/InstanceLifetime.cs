using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Enum-like class which indicates types of dependency instance lifetimes. This can
    /// be inherited to provide new types of scope.
    /// </summary>
    public class InstanceLifetime
    {
        /// <summary>
        /// A single instance is returned in the parent and all nested containers.
        /// </summary>
        public static readonly InstanceLifetime Singleton = new InstanceLifetime();

        /// <summary>
        /// A new instance will be returned each time.
        /// </summary>
        public static readonly InstanceLifetime Transient = new InstanceLifetime();

        /// <summary>
        /// This lifetime applies to nested scopes. A component with scoped lifetime will have at most a single 
        /// instance per nested scope. Equivalent to ServiceLifetime.Scoped in .net core.
        /// </summary>
        public static readonly InstanceLifetime Scoped = new InstanceLifetime();
    }
}
