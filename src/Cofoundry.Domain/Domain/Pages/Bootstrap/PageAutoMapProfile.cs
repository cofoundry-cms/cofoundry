using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class PageAutoMapProfile : Profile
    {
        public PageAutoMapProfile()
        {
            #region page

            CreateMap<Page, PageSummary>()
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.CustomEntityName, o => o.MapFrom(s => s.CustomEntityDefinition.EntityDefinition.Name))
                ;

            CreateMap<PageRoute, PageSummary>()
                ;

            CreateMap<Page, PageDetails>()
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                ;
            CreateMap<PageVersion, PageDetails>()
                .ForMember(d => d.Tags, o => o.MapFrom(s => s
                    .Page
                    .PageTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t))
                    )
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s.Page))
                .ForMember(d => d.LatestVersion, o => o.MapFrom(s => s))
                ;

            #endregion

            #region page versions

            CreateMap<PageVersion, PageRenderDetails>()
                .ForMember(d => d.MetaData, o => o.MapFrom(s => s))
                .ForMember(d => d.Template, o => o.MapFrom(s => s.PageTemplate))
                .ForMember(d => d.Sections, o => o.MapFrom(s => s.PageTemplate.PageTemplateSections))
                .ForMember(d => d.WorkFlowStatus, o => o.MapFrom(s => (Cofoundry.Domain.WorkFlowStatus)s.WorkFlowStatusId))
                ;

            CreateMap<PageVersion, PageVersionDetails>()
                .ForMember(d => d.Template, o => o.MapFrom(s => s.PageTemplate))
                .ForMember(d => d.WorkFlowStatus, o => o.MapFrom(s => (Cofoundry.Domain.WorkFlowStatus)s.WorkFlowStatusId))
                .ForMember(d => d.ShowInSiteMap, o => o.MapFrom(s => !s.ExcludeFromSitemap))
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.MetaData, o => o.MapFrom(s => s))
                .ForMember(d => d.OpenGraph, o => o.MapFrom(s => s))
                ;

            CreateMap<PageVersion, PageMetaData>()
                .ForMember(d => d.Description, o => o.MapFrom(s => s.MetaDescription))
                .ForMember(d => d.Keywords, o => o.MapFrom(s => s.MetaKeywords))
                ;

            CreateMap<PageVersion, OpenGraphData>()
                .ForMember(d => d.Description, o => o.MapFrom(s => s.OpenGraphDescription))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.OpenGraphTitle))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.OpenGraphImageAsset))
                ;

            #endregion

            #region page groups

            CreateMap<PageGroup, PageGroupMicroSummary>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.GroupName))
                ;

            CreateMap<PageGroup, PageGroupSummary>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.GroupName))
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.NumPages, o => o.MapFrom(s => s.PageGroupItems.Count()))
                ;

            #endregion

            #region modules

            CreateMap<PageModuleType, PageModuleTypeSummary>()
                .ForMember(d => d.Templates, o => o.MapFrom(s => s.PageModuleTemplates.OrderBy(t => t.Name)))
                ;

            CreateMap<PageModuleTypeSummary, PageModuleTypeDetails>();
            CreateMap<PageModuleTypeTemplate, PageModuleTypeTemplateSummary>();

            CreateMap<PageVersionModule, UpdatePageVersionModuleCommand>();

            #endregion

            #region templates

            CreateMap<PageTemplateSection, PageSectionRenderDetails>()
                ;

            #endregion

            #region commands

            CreateMap<Page, UpdatePageCommand>()
                .ForMember(d => d.Tags, o => o.MapFrom(s => s
                    .PageTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t))
                    )
                ;
            CreateMap<PageVersion, UpdatePageDraftVersionCommand>()
                .ForMember(d => d.ShowInSiteMap, o => o.MapFrom(s => !s.ExcludeFromSitemap))
                ;

            #endregion
        }
    }
}
