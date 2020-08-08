using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Shared helper between add/update page block commands for updating the db model
    /// </summary>
    public class PageBlockCommandHelper : IPageBlockCommandHelper
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

        public PageBlockCommandHelper(
            CofoundryDbContext dbContext,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer
            )
        {
            _dbContext = dbContext;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
        }

        public async Task UpdateModelAsync(
            IPageVersionBlockDataModelCommand command, 
            IEntityVersionPageBlock dbBlock
            )
        {
            if (command.PageBlockTypeId != dbBlock.PageBlockTypeId)
            {
                var pageTemplateRegionId = dbBlock.PageTemplateRegion != null ? dbBlock.PageTemplateRegion.PageTemplateRegionId : dbBlock.PageTemplateRegionId;
                var pageBlockType = await _dbContext
                    .PageBlockTypes
                    .Where(m => m.PageBlockTypeId == command.PageBlockTypeId)
                    .SingleOrDefaultAsync();

                EntityNotFoundException.ThrowIfNull(pageBlockType, command.PageBlockTypeId);
                dbBlock.PageBlockType = pageBlockType;
            }

            dbBlock.SerializedData = _dbUnstructuredDataSerializer.Serialize(command.DataModel);

            if (command.PageBlockTypeTemplateId != dbBlock.PageBlockTypeTemplateId && command.PageBlockTypeTemplateId.HasValue)
            {
                dbBlock.PageBlockTypeTemplate = await _dbContext
                    .PageBlockTypeTemplates
                    .SingleOrDefaultAsync(m => m.PageBlockTypeId == command.PageBlockTypeId && m.PageBlockTypeTemplateId == command.PageBlockTypeTemplateId);
                EntityNotFoundException.ThrowIfNull(dbBlock.PageBlockTypeTemplate, command.PageBlockTypeTemplateId);
            }
            else if (command.PageBlockTypeTemplateId != dbBlock.PageBlockTypeTemplateId)
            {
                dbBlock.PageBlockTypeTemplate = null;
            }
        }
    }
}
