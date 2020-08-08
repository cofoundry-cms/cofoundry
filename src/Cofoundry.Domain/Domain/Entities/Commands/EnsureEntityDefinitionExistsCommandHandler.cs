using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class EnsureEntityDefinitionExistsCommandHandler 
        : ICommandHandler<EnsureEntityDefinitionExistsCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityDefinitionRepository _entityDefinitionRepository;

        public EnsureEntityDefinitionExistsCommandHandler(
            CofoundryDbContext dbContext,
            IEntityDefinitionRepository entityDefinitionRepository
            )
        {
            _dbContext = dbContext;
            _entityDefinitionRepository = entityDefinitionRepository;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(EnsureEntityDefinitionExistsCommand command, IExecutionContext executionContext)
        {
            var entityDefinition = _entityDefinitionRepository.GetByCode(command.EntityDefinitionCode);

            var dbDefinition = await _dbContext
                .EntityDefinitions
                .SingleOrDefaultAsync(e => e.EntityDefinitionCode == command.EntityDefinitionCode);

            if (dbDefinition == null)
            {
                dbDefinition = new EntityDefinition()
                {
                    EntityDefinitionCode = entityDefinition.EntityDefinitionCode,
                    Name = entityDefinition.Name
                };

                _dbContext.EntityDefinitions.Add(dbDefinition);
                await _dbContext.SaveChangesAsync();
            }
        }

        #endregion
    }
}
