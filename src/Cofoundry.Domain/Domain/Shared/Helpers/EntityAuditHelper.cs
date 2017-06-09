using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class EntityAuditHelper
    {
        public void SetCreated(ICreateAuditable entity, IExecutionContext executionContext)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
            if (executionContext.UserContext.UserId == null)
            {
                throw new NotPermittedException("User must be logged in to update an ICreateAuditable entity");
            }

            entity.CreateDate = executionContext.ExecutionDate;
            entity.CreatorId = executionContext.UserContext.UserId.Value;

            if (entity is IUpdateAuditable)
            {
                SetUpdated((IUpdateAuditable)entity, executionContext);
            }
        }

        public void SetUpdated(IUpdateAuditable entity, IExecutionContext executionContext)
        {
            const string msg = "Cannot set an entity as updated if it has not yet been created. Property not set: ";

            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));
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
            entity.UpdaterId= executionContext.UserContext.UserId.Value;
        }
    }
}
