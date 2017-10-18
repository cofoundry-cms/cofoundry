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
            CreateMap<PageDirectoryLocale, PageDirectoryRouteLocale>();
            CreateMap<PageDirectory, UpdatePageDirectoryCommand>();
        }
    }
}
