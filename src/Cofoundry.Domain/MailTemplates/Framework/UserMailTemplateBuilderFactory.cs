using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.MailTemplates.AdminMailTemplates;
using Cofoundry.Domain.MailTemplates.DefaultMailTemplates;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.MailTemplates
{
    public class UserMailTemplateBuilderFactory : IUserMailTemplateBuilderFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPasswordResetUrlHelper _passwordResetUrlHelper;

        public UserMailTemplateBuilderFactory(
            IServiceProvider serviceProvider,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPasswordResetUrlHelper passwordResetUrlHelper
            )
        {
            _serviceProvider = serviceProvider;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _passwordResetUrlHelper = passwordResetUrlHelper;
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

            if (userAreaDefinition is CofoundryAdminUserArea)
            {
                // create the default Cofoundry admin builder
                var cofoundryAdminMailTemplateBuilder = _serviceProvider.GetRequiredService<ICofoundryAdminMailTemplateBuilder>();

                return new CofoundryAdminMailTemplateBuilderWrapper(cofoundryAdminMailTemplateBuilder);
            }

            var defaultBuilderType = typeof(DefaultMailTemplateBuilderWrapper<>).MakeGenericType(definitionType);

            // for other user areas fall back to the default builder
            return (IUserMailTemplateBuilder)_serviceProvider.GetService(defaultBuilderType);
        }
    }
}
