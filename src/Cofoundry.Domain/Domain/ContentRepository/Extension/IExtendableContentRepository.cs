using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Extendable
{
    public interface IExtendableContentRepository : IAdvancedContentRepository
    {
        /// <summary>
        /// Service provider instance only to be used for
        /// extending the ContentRepository with extension methods
        /// </summary>
        IServiceProvider ServiceProvider { get; }
    }
}
