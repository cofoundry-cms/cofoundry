using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public class SettingsAutoMapProfile : Profile
    {
        public SettingsAutoMapProfile()
        {
            #region commands

            CreateMap<GeneralSiteSettings, UpdateGeneralSiteSettingsCommand>();
            CreateMap<SeoSettings, UpdateSeoSettingsCommand>();
            CreateMap<SiteViewerSettings, UpdateSiteViewerSettingsCommand>();

            #endregion
        }
    }
}
