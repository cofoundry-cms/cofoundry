using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class LocaleAutoMapProfile : Profile
    {
        public LocaleAutoMapProfile()
        {
            CreateMap<Locale, ActiveLocale>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.LocaleName))
                ;
        }
    }
}
