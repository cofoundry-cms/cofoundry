using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class DataDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterType<CofoundryDbContext>(new Type[] { typeof(CofoundryDbContext), typeof(DbContext) })
                .RegisterType<IFileStoreService, FileSystemFileStoreService>()
                .RegisterType<IDbUnstructuredDataSerializer, DbUnstructuredDataSerializer>();
                ;
        }
    }
}
