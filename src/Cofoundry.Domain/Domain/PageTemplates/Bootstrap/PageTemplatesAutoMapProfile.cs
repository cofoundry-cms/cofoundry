using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class PageTemplatesAutoMapProfile : Profile
    {
        public PageTemplatesAutoMapProfile()
        {
            #region page Templates

            CreateMap<PageTemplate, PageTemplateMicroSummary>()
                .ForMember(d => d.CustomEntityModelType, o => o.ResolveUsing<PageTemplateCustomEntityTypeValueResolver, string>(s => s.CustomEntityModelType))
                ;
            
            CreateMap<PageTemplate, PageTemplateSummary>()
                .ForMember(d => d.PageType, o => o.MapFrom(s => (PageType)s.PageTypeId))
                .ForMember(d => d.NumPages, o => o.MapFrom(s => s
                    .PageVersions
                    .GroupBy(p => p.PageId)
                    .Count()))
                .ForMember(d => d.NumSections, o => o.MapFrom(s => s
                    .PageTemplateSections
                    .Count()))
                ;

            CreateMap<PageTemplate, PageTemplateDetails>()
                .ForMember(d => d.NumPages, o => o.MapFrom(s => s
                    .PageVersions
                    .GroupBy(p => p.PageId)
                    .Count()))
                .ForMember(d => d.PageType, o => o.MapFrom(s => (PageType)s.PageTypeId))
                .ForMember(d => d.Sections, o => o.MapFrom(s => s.PageTemplateSections))
                ;

            CreateMap<PageTemplateSection, PageTemplateSectionDetails>();
            
            #endregion
        }
    }
}
