//using Cofoundry.Core.DependencyInjection;
//using RazorEngine.Templating;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Cofoundry.Core.Mail
//{
//    /// <summary>
//    /// A DI reolver for RazorEngine templates
//    /// </summary>
//    public class RazorEngineActivator : IActivator
//    {
//        private readonly IResolutionContext _resolutionContext;

//        public RazorEngineActivator(
//            IResolutionContext resolutionContext
//            )
//        {
//            _resolutionContext = resolutionContext;
//        }

//        public ITemplate CreateInstance(InstanceContext context)
//        {
//            var instance = context.Loader.CreateInstance(context.TemplateType);
            
//            // Just doing some simple building up of the url helper. Could do something more scaleable if here if required.
//            if (instance is IEmailTemplateWithUrlHelper)
//            {
//                ((IEmailTemplateWithUrlHelper)instance).Url = _resolutionContext.Resolve<RazorEngineUrlHelper>();
//            }

//            return instance;
//        }
//    }
//}
