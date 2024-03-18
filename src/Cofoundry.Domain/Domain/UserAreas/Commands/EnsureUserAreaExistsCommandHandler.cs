﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class EnsureUserAreaExistsCommandHandler
    : ICommandHandler<EnsureUserAreaExistsCommand>
    , IIgnorePermissionCheckHandler
{
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

    public async Task ExecuteAsync(EnsureUserAreaExistsCommand command, IExecutionContext executionContext)
    {
        var userArea = _userAreaRepository.GetRequiredByCode(command.UserAreaCode);
        EntityNotFoundException.ThrowIfNull(userArea, command.UserAreaCode);

        var dbUserArea = await _dbContext
            .UserAreas
            .SingleOrDefaultAsync(a => a.UserAreaCode == userArea.UserAreaCode);

        if (dbUserArea == null)
        {
            dbUserArea = new UserArea
            {
                UserAreaCode = userArea.UserAreaCode,
                Name = userArea.Name
            };

            _dbContext.UserAreas.Add(dbUserArea);
        }
    }
}
