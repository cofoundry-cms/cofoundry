using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class UserAreaAutoMapProfile : Profile
    {
        public UserAreaAutoMapProfile()
        {
            CreateMap<UserArea, UserAreaMicroSummary>();
            CreateMap<IUserArea, UserAreaMicroSummary>();
            
        }
    }
}
