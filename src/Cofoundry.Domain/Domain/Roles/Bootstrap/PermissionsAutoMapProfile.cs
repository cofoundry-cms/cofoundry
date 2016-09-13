using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class PermissionsAutoMapProfile : Profile
    {
        public PermissionsAutoMapProfile()
        {
            CreateMap<Role, RoleDetails>()
                .ForMember(d => d.Permissions, o => o.Ignore())
                .ForMember(d => d.IsAnonymousRole, o => o.MapFrom(s => s.SpecialistRoleTypeCode == SpecialistRoleTypeCodes.Anonymous))
                .ForMember(d => d.IsSuperAdministrator, o => o.MapFrom(s => s.SpecialistRoleTypeCode == SpecialistRoleTypeCodes.SuperAdministrator))
                ;

            CreateMap<Role, RoleMicroSummary>();

            CreateMap<RoleDetails, UpdateRoleCommand>();
            CreateMap<IPermission, PermissionCommandData>()
                .ForMember(d => d.PermissionCode, o => o.MapFrom(s => s.PermissionType.Code))
                .ForMember(d => d.EntityDefinitionCode, o => o.MapFrom(s => s is IEntityPermission ? ((IEntityPermission)s).EntityDefinition.EntityDefinitionCode : null))
                ;
            
        }
    }
}
