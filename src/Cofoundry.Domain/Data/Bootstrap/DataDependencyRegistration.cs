using Cofoundry.Core.DependencyInjection;
using Cofoundry.Domain.Data.Internal;

namespace Cofoundry.Domain.Data.Registration;

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
            .Register<IPageDirectoryStoredProcedures, PageDirectoryStoredProcedures>()
            .Register<IUserStoredProcedures, UserStoredProcedures>()
            .Register<IAuthorizedTaskStoredProcedures, AuthorizedTaskStoredProcedures>()
            .Register<IIPAddressStoredProcedures, IPAddressStoredProcedures>()
            ;
    }
}
