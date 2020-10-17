using Cofoundry.Core.Data.Internal;
using Cofoundry.Core.Data.SimpleDatabase;
using Cofoundry.Core.Data.SimpleDatabase.Internal;
using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data.Registration
{
    public class DataTimeDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterScoped<ICofoundryDbConnectionManager, CofoundryDbConnectionManager>()
                .RegisterScoped<ITransactionScopeManager, DefaultTransactionScopeManager>()
                .RegisterScoped<ICofoundryDatabase, CofoundrySqlDatabase>()
                .RegisterScoped<ITransactionScopeFactory, TransactionScopeFactory>()
                ;
        }
    }
}
