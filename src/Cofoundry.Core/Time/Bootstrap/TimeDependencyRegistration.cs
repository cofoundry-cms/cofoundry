using Cofoundry.Core.DependencyInjection;
using Cofoundry.Core.Time.Internal;

namespace Cofoundry.Core.Time.Registration;

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
