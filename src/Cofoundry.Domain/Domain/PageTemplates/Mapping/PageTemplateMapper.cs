using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateMapper : IPageTemplateMapper
    {
        private readonly IPageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

        public PageTemplateMapper(
            IPageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper
            )
        {
            _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
        }

        public PageTemplateMicroSummary MapMicroSummary(PageTemplate dbPageTemplate)
        {
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
