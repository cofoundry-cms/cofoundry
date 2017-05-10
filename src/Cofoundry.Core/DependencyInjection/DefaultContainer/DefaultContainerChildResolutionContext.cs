using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    public class DefaultContainerChildResolutionContext : DefaultContainerResolutionContext, IChildResolutionContext
    {
        private readonly IServiceScope _serviceScope;

        public DefaultContainerChildResolutionContext(
            IServiceScope serviceScope
            ) 
            : base(serviceScope.ServiceProvider)
        {
            _serviceScope = serviceScope;
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _serviceScope.Dispose();
            }
        }

        #endregion
    }
}
