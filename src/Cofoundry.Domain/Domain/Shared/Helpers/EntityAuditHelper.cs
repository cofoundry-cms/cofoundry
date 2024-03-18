﻿using Cofoundry.Domain.Data;

namespace Cofoundry.Domain;

public class EntityAuditHelper
{
    public void SetCreated(ICreateAuditable entity, IExecutionContext executionContext)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(executionContext);

        if (executionContext.UserContext.UserId == null)
        {
            throw new NotPermittedException("User must be logged in to update an ICreateAuditable entity");
        }

        entity.CreateDate = executionContext.ExecutionDate;
        entity.CreatorId = executionContext.UserContext.UserId.Value;

        if (entity is IUpdateAuditable updateAuditable)
        {
            SetUpdated(updateAuditable, executionContext);
        }
    }

    public void SetUpdated(IUpdateAuditable entity, IExecutionContext executionContext)
    {
        const string msg = "Cannot set an entity as updated if it has not yet been created. Property not set: ";

        ArgumentNullException.ThrowIfNull(entity);
        ArgumentNullException.ThrowIfNull(executionContext);

        if (executionContext.UserContext.UserId == null)
        {
            throw new NotPermittedException("User must be logged in to update an ICreateAuditable entity");
        }

        if (entity.CreateDate == DateTime.MinValue)
        {
            throw new InvalidOperationException(msg + nameof(entity.CreateDate));
        }

        if (entity.CreatorId < 1)
        {
            throw new InvalidOperationException(msg + nameof(entity.CreatorId));
        }

        entity.UpdateDate = executionContext.ExecutionDate;
        entity.UpdaterId = executionContext.UserContext.UserId.Value;
    }
}
