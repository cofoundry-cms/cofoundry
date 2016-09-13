using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class UsersAutoMapProfile : Profile
    {
        public UsersAutoMapProfile()
        {
            CreateMap<User, UserMicroSummary>();

            CreateMap<User, UserLoginInfo>();
            CreateMap<User, UserContext>()
                .ForMember(d => d.UserArea, o => o.Ignore())
                .ForMember(d => d.IsPasswordChangeRequired, o => o.MapFrom(d => d.RequirePasswordChange));
                ;
            
            CreateMap<User, UserSummary>()
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s));

            CreateMap<User, UserDetails>()
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s));

            CreateMap<User, UserAccountDetails>()
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s));

            // Custom mapping here becuase users have a nullable CreatorId
            CreateMap<User, CreateAuditData>();

            CreateMap<User, UpdateUserCommand>();
            CreateMap<User, UpdateCurrentUserAccountCommand>();
        }
    }
}
