using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public class PageModulesAutoMapBootstrap : Profile
    {
        public PageModulesAutoMapBootstrap()
        {
            CreateMap<ImageDataModel, ImageDisplayModel>();
            CreateMap<QuotationDataModel, QuotationDisplayModel>()
                .ForMember(d => d.Quotation, o => o.MapFrom(s => s.Quotation.Replace(System.Environment.NewLine, "<br />")))
                ;
            CreateMap<RawHtmlDataModel, RawHtmlDisplayModel>();
            CreateMap<SingleLineTextDataModel, SingleLineTextDisplayModel>();
            CreateMap<TextListDataModel, TextListDisplayModel>()
                .ForMember(d => d.TextListItems, o => o.MapFrom(s => s.TextList.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None)))
                ;
        }
    }
}