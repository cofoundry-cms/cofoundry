using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data.Registration
{
    public class DataDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .Register<CofoundryDbContext>(new Type[] { typeof(CofoundryDbContext), typeof(DbContext) }, RegistrationOptions.Scoped())
                .Register<IFileStoreService, FileSystemFileStoreService>()
                .Register<IDbUnstructuredDataSerializer, DbUnstructuredDataSerializer>()
                .Register<ICustomEntityStoredProcedures, CustomEntityStoredProcedures>()
                .Register<IPageStoredProcedures, PageStoredProcedures>()
                .Register<IAssetStoredProcedures, AssetStoredProcedures>()
                ;
        }
    }
}
