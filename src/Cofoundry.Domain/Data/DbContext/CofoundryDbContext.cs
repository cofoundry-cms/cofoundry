using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cofoundry.Domain.Data
{
    public partial class CofoundryDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly DatabaseSettings _databaseSettings;

        public CofoundryDbContext(
            ILoggerFactory loggerFactory,
            DatabaseSettings databaseSettings
            )
        {
            _loggerFactory = loggerFactory;
            _databaseSettings = databaseSettings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(_loggerFactory);
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Log(RelationalEventId.QueryClientEvaluationWarning)
            );

            optionsBuilder.UseSqlServer(_databaseSettings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseDefaultConfig(DbConstants.CofoundrySchema)
                .MapCofoundryContent()
                .Map(new SettingMap())
                .Map(new RewriteRuleMap())
                ;
        }

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
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageDirectory> PageDirectories { get; set; }
        public DbSet<PageDirectoryLocale> PageDirectoryLocales { get; set; }
        public DbSet<PageModuleTypeTemplate> PageModuleTypeTemplates { get; set; }
        public DbSet<PageModuleType> PageModuleTypes { get; set; }
        public DbSet<PageGroupItem> PageGroupItems { get; set; }
        public DbSet<PageGroup> PageGroups { get; set; }
        public DbSet<PageVersionModule> PageVersionModules { get; set; }
        public DbSet<PageTag> PageTags { get; set; }
        public DbSet<PageVersion> PageVersions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RewriteRule> RewriteRules { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserArea> UserAreas { get; set; }
        public DbSet<UserPasswordResetRequest> UserPasswordResetRequests { get; set; }
        public DbSet<UnstructuredDataDependency> UnstructuredDataDependencies { get; set; }

        #endregion
    }
}
