﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

public class UserContextMapper
{
    private readonly IUserAreaDefinitionRepository _userAreaRepository;

    public UserContextMapper(
        IUserAreaDefinitionRepository userAreaRepository
        )
    {
        _userAreaRepository = userAreaRepository;
    }

    [return: NotNullIfNotNull(nameof(dbUser))]
    public UserContext? Map(User dbUser)
    {
        if (dbUser == null) return null;

        if (dbUser.Role == null)
        {
            throw new ArgumentException("User role is null. Ensure this has been included in the query.", nameof(dbUser));
        }

        var cx = new UserContext();

        cx.IsPasswordChangeRequired = dbUser.RequirePasswordChange;
        cx.IsAccountVerified = dbUser.AccountVerifiedDate.HasValue;
        cx.RoleId = dbUser.RoleId;
        cx.RoleCode = dbUser.Role.RoleCode;
        cx.UserId = dbUser.UserId;
        cx.UserArea = _userAreaRepository.GetRequiredByCode(dbUser.UserAreaCode);

        return cx;
    }
}
