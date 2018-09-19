using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class AdminRouteLibrary : IAdminRouteLibrary
    {
        public AdminRouteLibrary(AdminSettings adminSettings)
        {
            Account = new AccountRouteLibrary(adminSettings);
            Auth = new AuthRouteLibrary(adminSettings);
            CustomEntities = new CustomEntitiesRouteLibrary(adminSettings);
            Dashboard = new DashboardRouteLibrary(adminSettings);
            Directories = new DirectoriesRouteLibrary(adminSettings);
            Documents = new DocumentsModuleRouteLibrary(adminSettings);
            Images = new ImagesModuleRouteLibrary(adminSettings);
            Pages = new PagesModuleRouteLibrary(adminSettings);
            PageTemplates = new PageTemplatesRouteLibrary(adminSettings);
            Roles = new RolesRouteLibrary(adminSettings);
            Settings = new SettingsRouteLibrary(adminSettings);
            Setup = new SetupRouteLibrary(adminSettings);
            Shared = new SharedRouteLibrary(adminSettings);
            SharedAlternate = new SharedAlternateRouteLibrary(adminSettings);
            SharedPlugin = new SharedPluginRouteLibrary(adminSettings);
            Users = new UsersRouteLibrary(adminSettings);
            VisualEditor = new VisualEditorRouteLibrary(adminSettings);
        }

        public AccountRouteLibrary Account { get; private set; }
        public AuthRouteLibrary Auth { get; private set; }
        public CustomEntitiesRouteLibrary CustomEntities { get; private set; }
        public DashboardRouteLibrary Dashboard { get; private set; }
        public DirectoriesRouteLibrary Directories { get; private set; }
        public DocumentsModuleRouteLibrary Documents { get; private set; }
        public ImagesModuleRouteLibrary Images { get; private set; }
        public PagesModuleRouteLibrary Pages { get; private set; }
        public PageTemplatesRouteLibrary PageTemplates { get; private set; }
        public RolesRouteLibrary Roles { get; private set; }
        public SettingsRouteLibrary Settings { get; private set; }
        public SetupRouteLibrary Setup { get; private set; }
        public SharedRouteLibrary Shared { get; private set; }
        public SharedAlternateRouteLibrary SharedAlternate { get; private set; }
        public SharedPluginRouteLibrary SharedPlugin { get; private set; }
        public UsersRouteLibrary Users { get; private set; }
        public VisualEditorRouteLibrary VisualEditor { get; private set; }
    }
}