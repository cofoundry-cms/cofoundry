using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class PasswordPolicyFactory : IPasswordPolicyFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public PasswordPolicyFactory(
            IServiceProvider serviceProvider,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _serviceProvider = serviceProvider;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public IPasswordPolicy Create(string userAreaCode)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);
            return Create(userArea);
        }

        public IPasswordPolicy Create(IUserAreaDefinition userArea)
        {
            // See if a custom user area configuration has been implemented
            var userAreaConfigurationType = typeof(IPasswordPolicyConfiguration<>).MakeGenericType(userArea.GetType());
            var configuration = _serviceProvider.GetService(userAreaConfigurationType) as IPasswordPolicyConfigurationBase;

            if (configuration == null)
            {
                // Get Default
                configuration = _serviceProvider.GetRequiredService<IDefaultPasswordPolicyConfiguration>();
            }

            var options = _userAreaDefinitionRepository.GetOptionsByCode(userArea.UserAreaCode);
            var builder = new PasswordPolicyBuilder(_serviceProvider, options.Password);
            configuration.Configure(builder);

            return builder.Build();
        }
    }
}
