using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class PageBlockTypeAutoMapProfile : Profile
    {
        public PageBlockTypeAutoMapProfile()
        {
            CreateMap<PageBlockType, PageBlockTypeSummary>()
                .ForMember(d => d.Templates, o => o.MapFrom(s => s.PageBlockTemplates.OrderBy(t => t.Name)))
                ;

            CreateMap<PageBlockTypeSummary, PageBlockTypeDetails>();
            CreateMap<PageBlockTypeTemplate, PageBlockTypeTemplateSummary>();
        }
    }
}
