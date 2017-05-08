using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Conditions;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A mapping helper containing a couple of mapping methods used in multiple queires
    /// to map page modules in regular pages as well as custom entity details pages.
    /// </summary>
    public class EntityVersionPageModuleMapper : IEntityVersionPageModuleMapper
    {
        #region constructor

        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;

        public EntityVersionPageModuleMapper(
            IQueryExecutor queryExecutor,
            IPageVersionModuleModelMapper pageVersionModuleModelMapper
            )
        {
            _pageVersionModuleModelMapper = pageVersionModuleModelMapper;
        }

        #endregion

        #region private mapping class

        private class MappedPageModule
        {
            public IEntityVersionPageModule PageModule { get; set; }
            public IPageModuleDisplayModel DisplayModel { get; set; }
            public PageModuleTypeSummary ModuleType { get; set; }
        }

        #endregion

        #region public methods

        public async Task MapSectionsAsync<TModuleRenderDetails>(
            IEnumerable<IEntityVersionPageModule> dbModules,
            IEnumerable<IEntitySectionRenderDetails<TModuleRenderDetails>> sections,
            IEnumerable<PageModuleTypeSummary> allModuleTypes,
            WorkFlowStatusQuery workflowStatus
            )
            where TModuleRenderDetails : IEntityVersionPageModuleRenderDetails, new()
        {
            var mappedModules = await ToModuleMappingDataAsync(dbModules, allModuleTypes, workflowStatus);

            // Map Sections

            foreach (var section in sections)
            {
                var sectionMappedModules = mappedModules
                    .Where(m => m.PageModule.PageTemplateSectionId == section.PageTemplateSectionId)
                    .OrderBy(m => m.PageModule.Ordering);

                section.Modules = ToModuleRenderDetails<TModuleRenderDetails>(sectionMappedModules).ToArray();
            }
        }

        /// <summary>
        /// Locates and returns the correct templates for a module if it a custom template 
        /// assigned, otherwise null is returned.
        /// </summary>
        /// <param name="pageModule">An unmapped database module to locate the template for.</param>
        /// <param name="moduleType">The module type associated with the module in which to look for the template.</param>
        public PageModuleTypeTemplateSummary GetCustomTemplate(IEntityVersionPageModule pageModule, PageModuleTypeSummary moduleType)
        {
            Condition.Requires(pageModule, nameof(pageModule)).IsNotNull();
            Condition.Requires(pageModule, nameof(moduleType)).IsNotNull();

            if (!pageModule.PageModuleTypeTemplateId.HasValue) return null;

            var template = moduleType
                .Templates
                .FirstOrDefault(t => t.PageModuleTypeTemplateId == pageModule.PageModuleTypeTemplateId);

            Debug.Assert(template != null, string.Format("The module template with id {0} could not be found for {1} {2}", pageModule.PageModuleTypeTemplateId, pageModule.GetType().Name, pageModule.GetVersionModuleId()));

            return template;
        }


        #endregion

        #region private methods

        private async Task<List<MappedPageModule>> ToModuleMappingDataAsync(
            IEnumerable<IEntityVersionPageModule> entityModules, 
            IEnumerable<PageModuleTypeSummary> moduleTypes, 
            WorkFlowStatusQuery workflowStatus
            )
        {
            var mappedModules = new List<MappedPageModule>();

            foreach (var group in entityModules.GroupBy(m => m.PageModuleTypeId))
            {
                var moduleType = moduleTypes.SingleOrDefault(t => t.PageModuleTypeId == group.Key);
                var mapperOutput = await _pageVersionModuleModelMapper.MapDisplayModelAsync(moduleType.FileName, group, workflowStatus);

                foreach (var module in group)
                {
                    var mappedModule = new MappedPageModule()
                    {
                        PageModule = module,
                        ModuleType = moduleType,
                        DisplayModel = mapperOutput.Single(o => o.VersionModuleId == module.GetVersionModuleId()).DisplayModel
                    };

                    mappedModules.Add(mappedModule);
                }
            }

            return mappedModules;
        }

        private IEnumerable<TModuleRenderDetails> ToModuleRenderDetails<TModuleRenderDetails>(IEnumerable<MappedPageModule> dbModules)
            where TModuleRenderDetails : IEntityVersionPageModuleRenderDetails, new()
        {
            int index = 0;
            int size = dbModules.Count();

            foreach (var dbModule in dbModules)
            {
                var module = new TModuleRenderDetails();

                module.EntityVersionPageModuleId = dbModule.PageModule.GetVersionModuleId();
                module.ModuleType = dbModule.ModuleType;
                module.DisplayModel = dbModule.DisplayModel;
                module.Template = GetCustomTemplate(dbModule.PageModule, dbModule.ModuleType);

                // Add any list context information.
                var displayData = module.DisplayModel as IListablePageModuleDisplayModel;

                if (displayData != null)
                {
                    displayData.ListContext = new ListablePageModuleRenderContext()
                    {
                        Index = index,
                        NumModules = size
                    };

                    index++;
                }

                yield return module;
            }
        }

        #endregion
    }
}
