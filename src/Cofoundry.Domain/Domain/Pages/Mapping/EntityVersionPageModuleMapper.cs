using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class EntityVersionPageModuleMapper : IEntityVersionPageModuleMapper
    {
        #region constructor

        private readonly IPageVersionModuleModelMapper _pageVersionModuleModelMapper;
        private readonly IQueryExecutor _queryExecutor;

        public EntityVersionPageModuleMapper(
            IQueryExecutor queryExecutor,
            IPageVersionModuleModelMapper pageVersionModuleModelMapper
            )
        {
            _queryExecutor = queryExecutor;
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

        public async Task MapSectionsAsync<TModuleRenderDetails>(IEnumerable<IEntityVersionPageModule> dbModules, IEnumerable<IEntitySectionRenderDetails<TModuleRenderDetails>> sections, WorkFlowStatusQuery workflowStatus, IExecutionContext executionContext)
            where TModuleRenderDetails : IEntityVersionPageModuleRenderDetails, new()
        {
            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>(executionContext);
            var mappedModules = ToModuleMappingData(dbModules, allModuleTypes, workflowStatus);

            // Map Sections

            foreach (var section in sections)
            {
                var sectionMappedModules = mappedModules
                    .Where(m => m.PageModule.PageTemplateSectionId == section.PageTemplateSectionId)
                    .OrderBy(m => m.PageModule.Ordering);

                section.Modules = ToModuleRenderDetails<TModuleRenderDetails>(sectionMappedModules).ToArray();
            }
        }


        #endregion

        #region private methods

        private List<MappedPageModule> ToModuleMappingData(IEnumerable<IEntityVersionPageModule> entityModules, IEnumerable<PageModuleTypeSummary> moduleTypes, WorkFlowStatusQuery workflowStatus)
        {
            var mappedModules = new List<MappedPageModule>();

            foreach (var group in entityModules.GroupBy(m => m.PageModuleTypeId))
            {
                var moduleType = moduleTypes.SingleOrDefault(t => t.PageModuleTypeId == group.Key);
                var mapperOutput = _pageVersionModuleModelMapper.MapDisplayModel(moduleType.FileName, group, workflowStatus);

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
                module.Template = GetCustomTemplate(dbModule.PageModule, module);

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

        public PageModuleTypeTemplateSummary GetCustomTemplate<TModuleRenderDetails>(IEntityVersionPageModule pageModule, TModuleRenderDetails moduleRenderDetails)
            where TModuleRenderDetails : IEntityVersionPageModuleRenderDetails, new()
        {
            if (!pageModule.PageModuleTypeTemplateId.HasValue) return null;

            var template = moduleRenderDetails
                .ModuleType
                .Templates
                .FirstOrDefault(t => t.PageModuleTypeTemplateId == pageModule.PageModuleTypeTemplateId);

            Debug.Assert(template != null, string.Format("The module template with id {0} could not be found for {1} {2}", pageModule.PageModuleTypeTemplateId, pageModule.GetType().Name, pageModule.GetVersionModuleId()));

            return template;
        }

        #endregion
    }
}
