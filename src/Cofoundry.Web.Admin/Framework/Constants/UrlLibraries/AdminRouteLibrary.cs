using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Admin
{
    public class AdminRouteLibrary : IAdminRouteLibrary
    {
        public AdminRouteLibrary()
        {
            Account = new AccountRouteLibrary();
            Auth = new AuthRouteLibrary();
            CustomEntities = new CustomEntitiesRouteLibrary();
            Dashboard = new DashboardRouteLibrary();
            Directories = new DirectoriesRouteLibrary();
            Documents = new DocumentsModuleRouteLibrary();
            Images = new ImagesModuleRouteLibrary();
            Pages = new PagesModuleRouteLibrary();
            PageTemplates = new PageTemplatesRouteLibrary();
            Roles = new RolesRouteLibrary();
            Settings = new SettingsRouteLibrary();
            Setup = new SetupRouteLibrary();
            Shared = new SharedRouteLibrary();
            SharedAlternate = new SharedAlternateRouteLibrary();
            SharedPlugin = new SharedPluginRouteLibrary();
            Users = new UsersRouteLibrary();
            VisualEditor = new VisualEditorRouteLibrary();
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