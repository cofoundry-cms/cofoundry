using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageTemplateMicroSummary objects.
    /// </summary>
    public class PageTemplateMicroSummaryMapper : IPageTemplateMicroSummaryMapper
    {
        private readonly IPageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

        public PageTemplateMicroSummaryMapper(
            IPageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper
            )
        {
            _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
        }

        /// <summary>
        /// Maps an EF PageTemplate record from the db into an PageTemplateMicroSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbPageTemplate">PageTemplate record from the database.</param>
        public virtual PageTemplateMicroSummary Map(PageTemplate dbPageTemplate)
        {
            if (dbPageTemplate == null) return null;

            var pageTemplate = new PageTemplateMicroSummary();
            pageTemplate.CustomEntityDefinitionCode = dbPageTemplate.CustomEntityDefinitionCode;
            pageTemplate.FullPath = dbPageTemplate.FullPath;
            pageTemplate.IsArchived = dbPageTemplate.IsArchived;
            pageTemplate.Name = dbPageTemplate.Name;
            pageTemplate.PageTemplateId = dbPageTemplate.PageTemplateId;

            pageTemplate.CustomEntityModelType = _pageTemplateCustomEntityTypeMapper.Map(dbPageTemplate.CustomEntityModelType);

            return pageTemplate;
        }
    }
}
