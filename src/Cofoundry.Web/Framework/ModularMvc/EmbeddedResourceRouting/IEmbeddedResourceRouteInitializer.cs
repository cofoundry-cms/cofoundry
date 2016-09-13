using Cofoundry.Core.EmbeddedResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Adds mvc routes for content paths registered via
    /// classes that inherit from IEmbeddedContentRouteRegistration
    /// </summary>
    public interface IEmbeddedResourceRouteInitializer
    {
        /// <summary>
        /// Creates the embedded resource routes appending them to the
        /// MVC route table.
        /// </summary>
        void Initialize();
    }
}
