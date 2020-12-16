using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Time.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Time.Registration
{
    public class TimeDependencyRegistration : IDependencyRegistration
    {
        public void Register(IContainerRegister container)
        {
            container
                .RegisterSingleton<IDateTimeService, DateTimeService>()
                .RegisterScoped<ITimeZoneConverter, TimeZoneConverter>()
                ;
        }
    }
}
