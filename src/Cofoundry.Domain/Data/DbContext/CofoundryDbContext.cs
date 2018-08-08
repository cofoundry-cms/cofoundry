using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Diagnostics;

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
                .ApplyConfiguration(new SettingMap())
                .ApplyConfiguration(new RewriteRuleMap())
                ;
        }

        #region properties

        public DbSet<CustomEntityDefinition> CustomEntityDefinitions { get; set; }

        public DbSet<CustomEntity> CustomEntities { get; set; }

        public DbSet<CustomEntityVersion> CustomEntityVersions { get; set; }

        public DbSet<CustomEntityVersionPageBlock> CustomEntityVersionPageBlocks { get; set; }

        /// <summary>
        /// Lookup cache used for quickly finding the correct version for a
        /// specific publish status query e.g. 'Latest', 'Published', 
        /// 'PreferPublished'. These records are generated when custom entities
        /// are published or unpublished.
        /// </summary>
        public DbSet<CustomEntityPublishStatusQuery> CustomEntityPublishStatusQueries { get; set; }

        public DbSet<EntityDefinition> EntityDefinitions { get; set; }

        public DbSet<DocumentAssetGroupItem> DocumentAssetGroupItems { get; set; }

        public DbSet<DocumentAssetGroup> DocumentAssetGroups { get; set; }

        public DbSet<DocumentAsset> DocumentAssets { get; set; }

        public DbSet<DocumentAssetTag> DocumentAssetTags { get; set; }

        public DbSet<ImageAssetGroupItem> ImageAssetGroupItems { get; set; }

        public DbSet<ImageAssetGroup> ImageAssetGroups { get; set; }

        public DbSet<ImageAsset> ImageAssets { get; set; }

        public DbSet<ImageAssetTag> ImageAssetTags { get; set; }

        /// <summary>
        /// A Page Template represents a physical view template file and is used
        /// by a Page to render out content. 
        /// </summary>
        public DbSet<PageTemplate> PageTemplates { get; set; }

        /// <summary>
        /// Each PageTemplate can have zero or more regions which are defined in the 
        /// template file using the CofoundryTemplate helper, 
        /// e.g. @Cofoundry.Template.Region("MyRegionName"). These regions represent
        /// areas where page blocks can be placed (i.e. insert content).
        /// </summary>
        public DbSet<PageTemplateRegion> PageTemplateRegions { get; set; }

        public DbSet<Locale> Locales { get; set; }

        /// <summary>
        /// Pages represent the dynamically navigable pages of your website. Each page uses a template 
        /// which defines the regions of content that users can edit. Pages are a versioned entity and 
        /// therefore have many page version records. At one time a page may only have one draft 
        /// version, but can have many published versions; the latest published version is the one that 
        /// is rendered when the page is published. 
        /// </summary>
        public DbSet<Page> Pages { get; set; }

        /// <summary>
        /// Represents a folder in the dynamic web page heirarchy. There is always a 
        /// single root directory.
        /// </summary>
        public DbSet<PageDirectory> PageDirectories { get; set; }

        public DbSet<PageDirectoryLocale> PageDirectoryLocales { get; set; }

        /// <summary>
        /// A block can optionally have display templates associated with it, 
        /// which will give the user a choice about how the data is rendered out
        /// e.g. 'Wide', 'Headline', 'Large', 'Reversed'. If no template is set then 
        /// the default view is used for rendering.
        /// </summary>
        public DbSet<PageBlockTypeTemplate> PageBlockTypeTemplates { get; set; }

        /// <summary>
        /// Page block types represent a type of content that can be inserted into a content 
        /// region of a page which could be simple content like 'RawHtml', 'Image' or 
        /// 'PlainText'. Custom and more complex block types can be defined by a 
        /// developer. Block types are typically created when the application
        /// starts up in the auto-update process.
        /// </summary>
        public DbSet<PageBlockType> PageBlockTypes { get; set; }

        public DbSet<PageGroupItem> PageGroupItems { get; set; }

        public DbSet<PageGroup> PageGroups { get; set; }

        public DbSet<PageTag> PageTags { get; set; }

        /// <summary>
        /// Pages are a versioned entity and therefore have many page version
        /// records. At one time a page may only have one draft version, but
        /// can have many published versions; the latest published version is
        /// the one that is rendered when the page is published. 
        /// </summary>
        public DbSet<PageVersion> PageVersions { get; set; }

        public DbSet<PageVersionBlock> PageVersionBlocks { get; set; }

        /// <summary>
        /// Lookup cache used for quickly finding the correct version for a
        /// specific publish status query e.g. 'Latest', 'Published', 
        /// 'PreferPublished'. These records are generated when pages
        /// are published or unpublished.
        /// </summary>
        public DbSet<PagePublishStatusQuery> PagePublishStatusQueries { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RewriteRule> RewriteRules { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<RolePermission> RolePermissions { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Represents the user in the Cofoundry custom identity system. Users can be partitioned into
        /// different 'User Areas' that enabled the identity system use by the Cofoundry administration area 
        /// to be reused for other purposes, but this isn't a common scenario and often there will only be the Cofoundry UserArea.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Users can be partitioned into different 'User Areas' that enabled the identity system use by the Cofoundry administration area 
        /// to be reused for other purposes, but this isn't a common scenario and often there will only be the Cofoundry UserArea. UserAreas
        /// are defined in code by defining an IUserAreaDefinition
        /// </summary>
        public DbSet<UserArea> UserAreas { get; set; }

        public DbSet<UserPasswordResetRequest> UserPasswordResetRequests { get; set; }

        /// <summary>
        /// Contains a record of a relation between one entitiy and another
        /// when it's defined in unstructured data. Also contains information on how deletions
        /// should cascade for the relationship.
        /// </summary>
        public DbSet<UnstructuredDataDependency> UnstructuredDataDependencies { get; set; }

        #endregion
    }
}
