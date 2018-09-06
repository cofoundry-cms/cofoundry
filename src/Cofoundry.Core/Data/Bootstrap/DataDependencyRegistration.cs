using Cofoundry.Core.Data.SimpleDatabase;
using Cofoundry.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Data
{
    public class DataDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterScoped<ICofoundryDbConnectionManager, CofoundryDbConnectionManager>()
                .RegisterScoped<ITransactionScopeManager, TransactionScopeManager>()
                .RegisterScoped<ICofoundryDatabase, CofoundrySqlDatabase>()
                ;
        }
    }
}
