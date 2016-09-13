using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class ImageAutoMapProfile : Profile
    {
        public ImageAutoMapProfile()
        {
            CreateMap<ImageAsset, ImageAssetSummary>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.FileDescription))
                .ForMember(d => d.FileSizeInBytes, o => o.MapFrom(s => s.FileSize))
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s
                    .ImageAssetTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t))
                    )
                ;

            CreateMap<ImageAsset, ImageAssetRenderDetails>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.FileDescription))
                ;
            
            CreateMap<ImageAsset, ImageAssetDetails>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.FileDescription))
                .ForMember(d => d.FileSizeInBytes, o => o.MapFrom(s => s.FileSize))
                .ForMember(d => d.AuditData, o => o.MapFrom(s => s))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s
                    .ImageAssetTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t))
                    )
                ;

            CreateMap<ImageAsset, UpdateImageAssetCommand>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.FileDescription))
                .ForMember(d => d.Tags, o => o.MapFrom(s => s
                    .ImageAssetTags
                    .Select(t => t.Tag.TagText)
                    .OrderBy(t => t))
                    )
                ;

            CreateMap<ImageAssetRenderDetails, ImageAssetFile>();
        }
    }
}
