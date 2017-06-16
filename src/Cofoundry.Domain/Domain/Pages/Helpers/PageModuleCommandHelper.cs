using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Shared helper between add/update page module commands for updating the db model
    /// </summary>
    public class PageModuleCommandHelper : IPageModuleCommandHelper
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

        public PageModuleCommandHelper(
            CofoundryDbContext dbContext,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer
            )
        {
            _dbContext = dbContext;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
        }

        public async Task UpdateModelAsync(IPageVersionModuleDataModelCommand command, IEntityVersionPageModule dbModule)
        {
            if (command.PageModuleTypeId != dbModule.PageModuleTypeId)
            {
                var pageTemplateSectionId = dbModule.PageTemplateSection != null ? dbModule.PageTemplateSection.PageTemplateSectionId : dbModule.PageTemplateSectionId;
                var pageModuleType = await _dbContext
                    .PageModuleTypes
                    .Where(m => m.PageModuleTypeId == command.PageModuleTypeId)
                    .SingleOrDefaultAsync();

                EntityNotFoundException.ThrowIfNull(pageModuleType, command.PageModuleTypeId);
                dbModule.PageModuleType = pageModuleType;
            }

            dbModule.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.DataModel);

            if (command.PageModuleTypeTemplateId != dbModule.PageModuleTypeTemplateId && command.PageModuleTypeTemplateId.HasValue)
            {
                dbModule.PageModuleTypeTemplate = await _dbContext
                    .PageModuleTypeTemplates
                    .SingleOrDefaultAsync(m => m.PageModuleTypeId == command.PageModuleTypeId && m.PageModuleTypeTemplateId == command.PageModuleTypeTemplateId);
                EntityNotFoundException.ThrowIfNull(dbModule.PageModuleTypeTemplate, command.PageModuleTypeTemplateId);
            }
            else if (command.PageModuleTypeTemplateId != dbModule.PageModuleTypeTemplateId)
            {
                dbModule.PageModuleTypeTemplate = null;
            }
        }
    }
}
