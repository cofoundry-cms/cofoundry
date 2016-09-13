using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class WebDirectoriesAutoMapProfile : Profile
    {
        public WebDirectoriesAutoMapProfile()
        {
            CreateMap<WebDirectory, WebDirectoryRoute>()
                .ForMember(d => d.LocaleVariations, o => o.MapFrom(s => s.WebDirectoryLocales))
                ;
            CreateMap<WebDirectory, WebDirectoryNode>()
                .ForMember(d => d.ParentWebDirectory, o => o.Ignore())
                .ForMember(d => d.ChildWebDirectories, o => o.Ignore())
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.NumPages, o => o.MapFrom(s => s.Pages.Count(p => !p.IsDeleted)))
                ;
            
            CreateMap<WebDirectoryLocale, WebDirectoryRouteLocale>()
                ;
            CreateMap<WebDirectory, UpdateWebDirectoryCommand>()
                ;
            
        }
    }
}
