using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class PageModuleAutoMapProfile : Profile
    {
        public PageModuleAutoMapProfile()
        {
            #region modules

            CreateMap<PageModuleType, PageModuleTypeSummary>()
                .ForMember(d => d.Templates, o => o.MapFrom(s => s.PageModuleTemplates.OrderBy(t => t.Name)))
                ;

            CreateMap<PageModuleTypeSummary, PageModuleTypeDetails>();
            CreateMap<PageModuleTypeTemplate, PageModuleTypeTemplateSummary>();

            #endregion
        }
    }
}
