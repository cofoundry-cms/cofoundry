using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Configuration;
using System.Data.Entity;

namespace Cofoundry.Domain.Data
{
    public class DataDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            var perRequestOptions = new RegistrationOptions() { InstanceScope = InstanceScope.PerLifetimeScope };

            container
                .RegisterType<CofoundryDbContext>(new Type[] { typeof(CofoundryDbContext), typeof(DbContext) }, perRequestOptions)
                .RegisterType<IFileStoreService, FileSystemFileStoreService>()
                .RegisterFactory<FileSystemFileStorageSettings, ConfigurationSettingsFactory<FileSystemFileStorageSettings>>()
                .RegisterType<IDbUnstructuredDataSerializer, DbUnstructuredDataSerializer>();
                ;
        }
    }
}
