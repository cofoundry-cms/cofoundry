using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Registration
{
    public class SharedDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var replacementRegistraionOptions = new RegistrationOptions() { ReplaceExisting = true };

            container
                .Register<EntityAuditHelper, EntityAuditHelper>()
                .Register<EntityTagHelper, EntityTagHelper>()
                .Register<EntityOrderableHelper, EntityOrderableHelper>()
                .Register<IContentRouteLibrary, ContentRouteLibrary>()
                .Register<IViewFileReader, ViewFileReader>()
                .Register<IAuditDataMapper, AuditDataMapper>()
                ;
        }
    }
}
