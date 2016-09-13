using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class CustomEntityAutoMapProfile : Profile
    {
        public CustomEntityAutoMapProfile()
        {
            CreateMap<CustomEntity, CustomEntityRoute>()
                .ForMember(d => d.Versions, o => o.Ignore())
                ;

            CreateMap<CustomEntityVersion, CustomEntityVersionRoute>()
                .ForMember(d => d.VersionId, o => o.MapFrom(s => s.CustomEntityVersionId))
                .ForMember(d => d.WorkFlowStatus, o => o.MapFrom(s => (Cofoundry.Domain.WorkFlowStatus)s.WorkFlowStatusId))
                ;

            CreateMap<CustomEntityVersion, CustomEntityRenderSummary>()
                .ForMember(d => d.CustomEntityDefinitionCode, o => o.MapFrom(s => s.CustomEntity.CustomEntityDefinitionCode))
                .ForMember(d => d.Locale, o => o.MapFrom(s => s.CustomEntity.Locale))
                .ForMember(d => d.UrlSlug, o => o.MapFrom(s => s.CustomEntity.UrlSlug))
                .ForMember(d => d.CreateDate, o => o.MapFrom(s => s.CustomEntity.CreateDate))
                .ForMember(d => d.WorkFlowStatus, o => o.MapFrom(s => (Cofoundry.Domain.WorkFlowStatus)s.WorkFlowStatusId))
                .ForMember(d => d.Ordering, o => o.MapFrom(s => s.CustomEntity.Ordering))
                ;
            
            CreateMap<CustomEntityVersion, CustomEntityRenderDetails>()
                .ForMember(d => d.CustomEntityDefinitionCode, o => o.MapFrom(s => s.CustomEntity.CustomEntityDefinitionCode))
                .ForMember(d => d.Locale, o => o.MapFrom(s => s.CustomEntity.Locale))
                .ForMember(d => d.UrlSlug, o => o.MapFrom(s => s.CustomEntity.UrlSlug))
                .ForMember(d => d.CreateDate, o => o.MapFrom(s => s.CustomEntity.CreateDate))
                .ForMember(d => d.WorkFlowStatus, o => o.MapFrom(s => (Cofoundry.Domain.WorkFlowStatus)s.WorkFlowStatusId))
                .ForMember(d => d.Ordering, o => o.MapFrom(s => s.CustomEntity.Ordering))
                ;
            
            CreateMap<PageTemplateSection, CustomEntityPageSectionRenderDetails>();
            
            CreateMap<CustomEntityVersion, CustomEntitySummaryQueryModel>()
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s.CustomEntity))
                .ForMember(d => d.VersionAuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.UrlSlug, o => o.MapFrom(s => s.CustomEntity.UrlSlug))
                .ForMember(d => d.LocaleId, o => o.MapFrom(s => s.CustomEntity.LocaleId))
                .ForMember(d => d.Ordering, o => o.MapFrom(s => s.CustomEntity.Ordering))
                .ForMember(d => d.CustomEntityDefinitionCode, o => o.MapFrom(s => s.CustomEntity.CustomEntityDefinitionCode))
                .ForMember(d => d.HasDraft, o => o.MapFrom(s => s.WorkFlowStatusId == (int)WorkFlowStatus.Draft))
                .ForMember(d => d.IsPublished, o => o.MapFrom(s => s.WorkFlowStatusId == (int)WorkFlowStatus.Published || s.CustomEntity.CustomEntityVersions.Any(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published)))
                ;
            CreateMap<CustomEntitySummaryQueryModel, CustomEntitySummary>();

            CreateMap<CustomEntityVersion, CustomEntityVersionSummary>()
                .ForMember(d => d.WorkFlowStatus, o => o.MapFrom(s => (Cofoundry.Domain.WorkFlowStatus)s.WorkFlowStatusId))
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                ;

            CreateMap<CustomEntity, CustomEntityDetails>()
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                ;

            CreateMap<CustomEntityVersion, CustomEntityVersionDetails>()
                .ForMember(d => d.WorkFlowStatus, o => o.MapFrom(s => (Cofoundry.Domain.WorkFlowStatus)s.WorkFlowStatusId))
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                ;

            
            CreateMap<PageRoutingInfo, CustomEntitySummary>()
                .ForMember(d => d.FullPath, o => o.MapFrom(s => s.CustomEntityRouteRule.MakeUrl(s.PageRoute, s.CustomEntityRoute)))
                ;

            #region definitions

            CreateMap<ICustomEntityDefinition, CustomEntityDefinitionSummary>()
                .ForMember(d => d.DataModelType, o => o.MapFrom(s => s.GetDataModelType()))
                .ForMember(d => d.Terms, o => o.MapFrom(s => s.GetTerms()))
                .ForMember(d => d.Ordering, o => o.MapFrom(s => s is IOrderableCustomEntityDefinition ? ((IOrderableCustomEntityDefinition)s).Ordering : CustomEntityOrdering.None))
                ;

            // This mapping is just to help with projection - get the actual entity using a command becuase
            // it is defined in code.
            CreateMap<CustomEntityDefinition, CustomEntityDefinitionMicroSummary>();
            CreateMap<CustomEntityDefinitionMicroSummary, CustomEntityDefinition>();

            CreateMap<CustomEntityDefinitionSummary, CustomEntityDefinitionMicroSummary>();
            CreateMap<ICustomEntityDefinition, CustomEntityDefinitionMicroSummary>();
            
            #endregion

            #region commands

            CreateMap<CustomEntityVersion, UpdateCustomEntityDraftVersionCommand>()
                .ForMember(d => d.CustomEntityDefinitionCode, o => o.MapFrom(s => s.CustomEntity.CustomEntityDefinitionCode))
                ;

            CreateMap<CustomEntityVersionPageModule, UpdateCustomEntityVersionPageModuleCommand>()
                ;
            
            #endregion
        }
    }
}
