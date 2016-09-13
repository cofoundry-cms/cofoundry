using Cofoundry.Core.DependencyInjection;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// A DI reolver for RazorEngine templates
    /// </summary>
    public class RazorEngineActivator : IActivator
    {
        private readonly IResolutionContext _resolutionContext;

        public RazorEngineActivator(
            IResolutionContext resolutionContext
            )
        {
            _resolutionContext = resolutionContext;
        }

        public ITemplate CreateInstance(InstanceContext context)
        {
            if (_resolutionContext.IsRegistered(context.TemplateType))
            {
                return (ITemplate)_resolutionContext.Resolve(context.TemplateType);
            }

            return context.Loader.CreateInstance(context.TemplateType);
        }
    }
}
