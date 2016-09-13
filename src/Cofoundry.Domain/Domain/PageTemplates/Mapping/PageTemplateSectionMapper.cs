using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class PageTemplateSectionMapper
    {
        private readonly IQueryExecutor _queryExecutor;

        public PageTemplateSectionMapper(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        public IEnumerable<PageTemplateSectionDetails> MapDetails(List<PageTemplateSection> dbSections)
        {
            var modules = GetModuleTypes();
            var genericModules = GetGenericModuleTypes(modules);

            var sections = new List<PageTemplateSectionDetails>(dbSections.Count);

            foreach (var dbSection in dbSections)
            {
                var section = Map(dbSection, modules, genericModules);
                sections.Add(section);
            }

            return sections;
        }

        public PageTemplateSectionDetails MapDetails(PageTemplateSection dbSection)
        {
            var modules = GetModuleTypes();
            var genericModules = GetGenericModuleTypes(modules);

            return Map(dbSection, modules, genericModules);
        }

        private IEnumerable<PageModuleTypeSummary> GetModuleTypes()
        {
            return _queryExecutor.GetAll<PageModuleTypeSummary>();
        }

        private PageModuleTypeSummary[] GetGenericModuleTypes(IEnumerable<PageModuleTypeSummary> moduleTypes)
        {
            return moduleTypes
                .Where(t => !t.IsCustom)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        private PageTemplateSectionDetails Map(PageTemplateSection dbSection, IEnumerable<PageModuleTypeSummary> moduleTypes, PageModuleTypeSummary[] genericModuleTypes)
        {
            var section = Mapper.Map<PageTemplateSectionDetails>(dbSection);

            if (dbSection.PageModuleTypes.Count == genericModuleTypes.Count())
            {
                section.ModuleTypes = genericModuleTypes;
                section.HasAllModuleTypes = true;
            }
            else
            {
                section.ModuleTypes = moduleTypes
                    .Where(t => dbSection.PageModuleTypes.Select(dbt => dbt.PageModuleTypeId).Contains(t.PageModuleTypeId))
                    .OrderBy(t => t.Name)
                    .ToArray();
            }

            return section;
        }
    }
}
