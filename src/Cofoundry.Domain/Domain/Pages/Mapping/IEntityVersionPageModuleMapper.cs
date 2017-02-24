using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A mapping helper containing a couple of mapping methods used in multiple queires
    /// to map page modules in regular pages as well as custom entity details pages.
    /// </summary>
    public interface IEntityVersionPageModuleMapper
    {
        Task MapSectionsAsync<TModuleRenderDetails>(IEnumerable<IEntityVersionPageModule> dbModules, IEnumerable<IEntitySectionRenderDetails<TModuleRenderDetails>> sections, WorkFlowStatusQuery workflowStatus, IExecutionContext executionContext)
            where TModuleRenderDetails : IEntityVersionPageModuleRenderDetails, new();

        /// <summary>
        /// Locates and returns the correct templates for a module if it a custom template 
        /// assigned, otherwise null is returned.
        /// </summary>
        /// <param name="pageModule">An unmapped database module to locate the template for.</param>
        /// <param name="moduleType">The module type associated with the module in which to look for the template.</param>
        PageModuleTypeTemplateSummary GetCustomTemplate(IEntityVersionPageModule pageModule, PageModuleTypeSummary moduleType);
    }
}
