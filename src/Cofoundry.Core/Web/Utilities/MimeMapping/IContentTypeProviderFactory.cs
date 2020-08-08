using Cofoundry.Core.DependencyInjection;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Web.Internal
{
    /// <summary>
    /// Factory for creating the IContentTypeProvider that gets registered 
    /// with the DI container as a single instance.
    /// </summary>
    public interface IContentTypeProviderFactory : IInjectionFactory<IContentTypeProvider>
    {
    }
}
