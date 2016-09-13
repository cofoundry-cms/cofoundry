using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    /// <summary>
    /// Enum-like class which indicates types of dependency lifetime scopes. This can
    /// be inherited to provide new types of scope.
    /// </summary>
    public class InstanceScope
    {
        /// <summary>
        /// A single instance is returned in the parent and all nested containers.
        /// </summary>
        public static readonly InstanceScope Singleton = new InstanceScope();

        /// <summary>
        /// A new instance will be returned each time.
        /// </summary>
        public static readonly InstanceScope Transient = new InstanceScope();

        /// <summary>
        /// This scope applies to nested lifetimes. A component with per-lifetime scope will have at most a single 
        /// instance per nested lifetime scope.
        /// </summary>
        public static readonly InstanceScope PerLifetimeScope = new InstanceScope();

        /// <summary>
        /// A new instance will be created per web request and will be returned in the parent 
        /// and all nested containers.
        /// </summary>
        public static readonly InstanceScope PerWebRequest = new InstanceScope();
    }
}
