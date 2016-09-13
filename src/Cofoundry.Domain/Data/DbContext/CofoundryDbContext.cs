using Cofoundry.Core;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;

namespace Cofoundry.Domain.Data
{
    public partial class CofoundryDbContext : DbContext
    {
        #region constructor

        static CofoundryDbContext()
        {
            Database.SetInitializer<CofoundryDbContext>(null);
        }

        public CofoundryDbContext()
            : base(DbConstants.ConnectionStringName)
        {
            DbContextConfigurationHelper.SetDefaults(this);
        }

        #endregion

        #region properties

        public DbSet<CustomEntityDefinition> CustomEntityDefinitions { get; set; }
        public DbSet<CustomEntity> CustomEntities { get; set; }
        public DbSet<CustomEntityVersion> CustomEntityVersions { get; set; }
        public DbSet<CustomEntityVersionPageModule> CustomEntityVersionPageModules { get; set; }
        public DbSet<EntityDefinition> EntityDefinitions { get; set; }
        public DbSet<DocumentAssetGroupItem> DocumentAssetGroupItems { get; set; }
        public DbSet<DocumentAssetGroup> DocumentAssetGroups { get; set; }
        public DbSet<DocumentAsset> DocumentAssets { get; set; }
        public DbSet<DocumentAssetTag> DocumentAssetTags { get; set; }
        public DbSet<ImageAssetGroupItem> ImageAssetGroupItems { get; set; }
        public DbSet<ImageAssetGroup> ImageAssetGroups { get; set; }
        public DbSet<ImageAsset> ImageAssets { get; set; }
        public DbSet<ImageAssetTag> ImageAssetTags { get; set; }
        public DbSet<PageTemplate> PageTemplates { get; set; }
        public DbSet<PageTemplateSection> PageTemplateSections { get; set; }
        public DbSet<Locale> Locales { get; set; }
        public DbSet<PageModuleTypeTemplate> PageModuleTypeTemplates { get; set; }
        public DbSet<PageModuleType> PageModuleTypes { get; set; }
        public DbSet<PageGroupItem> PageGroupItems { get; set; }
        public DbSet<PageGroup> PageGroups { get; set; }
        public DbSet<PageVersionModule> PageVersionModules { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageTag> PageTags { get; set; }
        public DbSet<PageVersion> PageVersions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RewriteRule> RewriteRules { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserArea> UserAreas { get; set; }
        public DbSet<UserPasswordResetRequest> UserPasswordResetRequests { get; set; }
        public DbSet<UnstructuredDataDependency> UnstructuredDataDependencies { get; set; }
        public DbSet<WebDirectory> WebDirectories { get; set; }
        public DbSet<WebDirectoryLocale> WebDirectoryLocales { get; set; }
        public DbSet<WorkFlowStatus> WorkFlowStatuses { get; set; }

        #endregion
        
        #region mapping

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .UseDefaultConfig(DbConstants.CofoundrySchema)
                .MapCofoundryUsers()
                .MapCofoundryTags()
                .MapCofoundryImageAssets()
                .MapCofoundryDocumentAssets()
                .MapCofoundryLocales()
                .MapCofoundryEntityDefinitions()
                .MapCofoundryCustomEntities()
                .Map(new CustomEntityVersionPageModuleMap())
                .Map(new PageTemplateMap())
                .Map(new PageTemplateSectionMap())
                .Map(new ModuleTemplateMap())
                .Map(new PageModuleTypeMap())
                .Map(new PageGroupItemMap())
                .Map(new PageGroupMap())
                .Map(new PageVersionModuleMap())
                .Map(new PageMap())
                .Map(new PageTagMap())
                .Map(new PageVersionMap())
                .Map(new RewriteRuleMap())
                .Map(new SettingMap())
                .Map(new UserPasswordResetRequestMap())
                .Map(new WebDirectoryMap())
                .Map(new WebDirectoryLocaleMap())
                .Map(new WorkFlowStatusMap());
        }

        #endregion
    }
}
