using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Web
{
    /// <summary>
    /// Pointer to various forms of dependency resolution using the Common Service
    /// Locator pattern. Use is strongly discouraged but it sometimes unavoidable because of
    /// the limitations of asp.net frameworks.
    /// </summary>
    public static class IckyDependencyResolution
    {
        /// <summary>
        /// Resolves an instance of the specified type from the root container. Normally
        /// you shouldn't be doing this and instead should manage a child context created from 
        /// CreateNewChildContextFromRoot and dispose of the child context when done.
        /// </summary>
        public static T ResolveFromRootContext<T>()
        {
            return (T)ServiceLocator.Current.GetService(typeof(T));
        }

        /// <summary>
        /// Create a new IChildResolutionContext to resolve dependencies in a
        /// new nested liftime. Must be disposed of after us.
        /// </summary>
        public static IChildResolutionContext CreateNewChildContextFromRoot()
        {
            var cx = (IResolutionContext)ServiceLocator.Current.GetService(typeof(IResolutionContext));
            return cx.CreateChildContext();
        }

        /// <summary>
        /// Resolves an instance from the MVC depdency resolve. This requires an mvc request context.
        /// </summary>
        public static T ResolveFromMvcContext<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        /// <summary>
        /// Resolves an instance from the MVC depdency resolve. This requires an mvc request context.
        /// </summary>
        public static object ResolveFromMvcContext(Type t)
        {
            return DependencyResolver.Current.GetService(t);
        }

        /// <summary>
        /// Resolves an instance from the MVC depdency resolve. This requires a web api request context.
        /// </summary>
        public static T ResolveInWebApiContext<T>()
        {
            return (T)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(T));
        }
    }
}
