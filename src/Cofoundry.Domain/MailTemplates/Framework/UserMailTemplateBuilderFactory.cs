using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.MailTemplates.AdminMailTemplates;
using Cofoundry.Domain.MailTemplates.GenericMailTemplates;
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
            var userAreaDefinition = _userAreaDefinitionRepository.GetByCode(userAreaDefinitionCode);
            EntityNotFoundException.ThrowIfNull(userAreaDefinition, userAreaDefinitionCode);

            // Try and find a factory registered for the specific user area
            var factoryType = typeof(IUserMailTemplateBuilder<>).MakeGenericType(userAreaDefinition.GetType());
            var factory = _serviceProvider.GetService(factoryType);

            if (factory != null) return (IUserMailTemplateBuilder)factory;

            var queryExecutor = _serviceProvider.GetRequiredService<IQueryExecutor>();
            if (userAreaDefinition is CofoundryAdminUserArea)
            {
                // create the default Cofoundry admin builder
                var adminMailTemplateUrlLibrary = _serviceProvider.GetRequiredService<AdminMailTemplateUrlLibrary>();

                return new CofoundryAdminMailTemplateBuilder(
                    queryExecutor,
                    adminMailTemplateUrlLibrary
                    );
            }

            // for other user areas fall back to the generic builder
            return new GenericMailTemplateBuilder(
                userAreaDefinition,
                queryExecutor
                );
        }
    }
}
