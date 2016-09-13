using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class SharedAutoMapProfile : Profile
    {
        public SharedAutoMapProfile()
        {
            CreateMap<ICreateAuditable, CreateAuditData>();
            CreateMap<IUpdateAuditable, UpdateAuditData>();
            CreateMap<CreateAuditData, UpdateAuditData>();
            CreateMap<UpdateAuditData, CreateAuditData>();
        }
    }
}
