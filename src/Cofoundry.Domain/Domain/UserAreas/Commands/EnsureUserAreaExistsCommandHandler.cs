using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class EnsureUserAreaExistsCommandHandler
        : ICommandHandler<EnsureUserAreaExistsCommand>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public EnsureUserAreaExistsCommandHandler(
            CofoundryDbContext dbContext,
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _dbContext = dbContext;
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        #region Execute

        public async Task ExecuteAsync(EnsureUserAreaExistsCommand command, IExecutionContext executionContext)
        {
            var userArea = _userAreaRepository.GetByCode(command.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, command.UserAreaCode);

            var dbUserArea = await _dbContext
                .UserAreas
                .SingleOrDefaultAsync(a => a.UserAreaCode == userArea.UserAreaCode);

            if (dbUserArea == null)
            {
                dbUserArea = new UserArea();
                dbUserArea.UserAreaCode = userArea.UserAreaCode;
                dbUserArea.Name = userArea.Name;

                _dbContext.UserAreas.Add(dbUserArea);
            }
        }

        #endregion
    }
}
