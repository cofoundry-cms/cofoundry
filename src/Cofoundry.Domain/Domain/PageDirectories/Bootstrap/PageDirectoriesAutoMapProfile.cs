using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class PageDirectoriesAutoMapProfile : Profile
    {
        public PageDirectoriesAutoMapProfile()
        {
            CreateMap<PageDirectory, PageDirectoryRoute>()
                .ForMember(d => d.LocaleVariations, o => o.MapFrom(s => s.PageDirectoryLocales))
                ;
            CreateMap<PageDirectory, PageDirectoryNode>()
                .ForMember(d => d.ParentPageDirectory, o => o.Ignore())
                .ForMember(d => d.ChildPageDirectories, o => o.Ignore())
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.NumPages, o => o.MapFrom(s => s.Pages.Count(p => !p.IsDeleted)))
                ;
            
            CreateMap<PageDirectoryLocale, PageDirectoryRouteLocale>()
                ;
            CreateMap<PageDirectory, UpdatePageDirectoryCommand>()
                ;
            
        }
    }
}
