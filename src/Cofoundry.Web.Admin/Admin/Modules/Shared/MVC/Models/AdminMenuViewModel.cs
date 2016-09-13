using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class AdminMenuViewModel
    {
        public IEnumerable<AdminModule> ManageSiteModules { get; set; }

        public IEnumerable<AdminModule> SettingsModules { get; set; }

        public AdminModuleMenuCategory SelectedCategory { get; set; }

        public AdminModule SelectedModule { get; set; }
    }
}