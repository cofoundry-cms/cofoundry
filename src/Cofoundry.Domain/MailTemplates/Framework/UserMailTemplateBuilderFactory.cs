using Cofoundry.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Cofoundry.Domain.MailTemplates.Internal
{
    public class UserMailTemplateBuilderFactory : IUserMailTemplateBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public UserMailTemplateBuilderFactory(
            IServiceProvider serviceProvider,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _serviceProvider = serviceProvider;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public IUserMailTemplateBuilder Create(string userAreaDefinitionCode)
        {
            var userAreaDefinition = _userAreaDefinitionRepository.GetRequiredByCode(userAreaDefinitionCode);
            EntityNotFoundException.ThrowIfNull(userAreaDefinition, userAreaDefinitionCode);

            // Try and find a factory registered for the specific user area
            var definitionType = userAreaDefinition.GetType();
            var factoryType = typeof(IUserMailTemplateBuilder<>).MakeGenericType(definitionType);
            var factory = _serviceProvider.GetService(factoryType);

            if (factory != null) return (IUserMailTemplateBuilder)factory;

            var defaultBuilderType = typeof(IDefaultUserMailTemplateBuilder<>).MakeGenericType(definitionType);

            // for other user areas fall back to the default builder
            return (IUserMailTemplateBuilder)_serviceProvider.GetRequiredService(defaultBuilderType);
        }
    }
}