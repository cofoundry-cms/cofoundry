using Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class EntityAuditHelper
    {
        public void SetCreated(ICreateAuditable entity, IExecutionContext executionContext)
        {
            Condition.Requires(entity).IsNotNull();
            Condition.Requires(executionContext).IsNotNull();
            Condition.Requires(executionContext.UserContext.UserId).IsNotNull("User must be logged in to update an ICreateAuditable entity");

            entity.CreateDate = executionContext.ExecutionDate;
            entity.CreatorId = executionContext.UserContext.UserId.Value;

            if (entity is IUpdateAuditable)
            {
                SetUpdated((IUpdateAuditable)entity, executionContext);
            }
        }

        public void SetUpdated(IUpdateAuditable entity, IExecutionContext executionContext)
        {
            Condition.Requires(entity).IsNotNull();
            Condition.Requires(entity.CreateDate).IsGreaterThan(DateTime.MinValue);
            Condition.Requires(entity.CreatorId).IsGreaterThan(0);
            Condition.Requires(executionContext).IsNotNull();
            Condition.Requires(executionContext.UserContext.UserId).IsNotNull("User must be logged in to update an ICreateAuditable entity");

            entity.UpdateDate = executionContext.ExecutionDate;
            entity.UpdaterId= executionContext.UserContext.UserId.Value;
        }
    }
}
