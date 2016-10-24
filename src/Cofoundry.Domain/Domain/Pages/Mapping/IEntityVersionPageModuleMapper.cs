using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public interface IEntityVersionPageModuleMapper
    {
        Task MapSectionsAsync<TModuleRenderDetails>(IEnumerable<IEntityVersionPageModule> dbModules, IEnumerable<IEntitySectionRenderDetails<TModuleRenderDetails>> sections, WorkFlowStatusQuery workflowStatus, IExecutionContext executionContext)
            where TModuleRenderDetails : IEntityVersionPageModuleRenderDetails, new();
    }
}
