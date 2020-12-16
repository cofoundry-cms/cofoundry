using System;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// The main Cofoundry entity framework DbContext representing all the main 
    /// entities in the Cofoundry database. Direct access to the DbContext is
    /// discouraged, instead we advise you use the domain queries and commands
    /// available in the Cofoundry data repositories, see
    /// https://github.com/cofoundry-cms/cofoundry/wiki/Data-Access#repositories
    /// </summary>
    public partial class CofoundryDbContext : DbContext
    {
        private readonly ICofoundryDbContextInitializer _cofoundryDbContextInitializer;

        public CofoundryDbContext(
            ICofoundryDbContextInitializer cofoundryDbContextInitializer
            )
        {
            _cofoundryDbContextInitializer = cofoundryDbContextInitializer;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _cofoundryDbContextInitializer.Configure(this, optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasDefaultSchema(DbConstants.CofoundrySchema)
                .MapCofoundryContent()
                .ApplyConfiguration(new SettingMap())
                .ApplyConfiguration(new RewriteRuleMap())
                ;
        }

        #region properties

        /// <summary>
        /// This queue keeps track of files belonging to assets that 
        /// have been deleted in the system. The files don't get deleted
        /// at the same time as the asset record and instead are queued
        /// and deleted by a background task to avoid issues with file
        /// locking and other errors that may cause the delete operation 
        /// to fail. It also enabled the file deletion to run in a 
        /// transaction.
        /// </summary>
        public DbSet<AssetFileCleanupQueueItem> AssetFileCleanupQueueItems { get; set; }

        /// <summary>
        /// <para>
        /// Custom entity definitions are used to define the identity and
        /// behavior of a custom entity type. This includes meta data such
        /// as the name and description, but also the configuration of
        /// features such as whether the identity can contain a locale
        /// and whether versioning (i.e. auto-publish) is enabled.
        /// </para>
        /// <para>
        /// Definitions are defined in code by implementing ICustomEntityDefinition
        /// but they are also stored in the database to help with queries and data 
        /// integrity.
        /// </para>
        /// <para>
        /// The code definition is the source of truth and the database is updated
        /// at runtime when an entity is added/updated. This is done via 
        /// EnsureCustomEntityDefinitionExistsCommand.
        /// </para>
        /// </summary>
        public DbSet<CustomEntityDefinition> CustomEntityDefinitions { get; set; }

        /// <summary>
        /// <para>
        /// Custom entities are a flexible system for developer defined
        /// data structures. The identity for these entities are persisted in
        /// this CustomEntity table, while the majority of data is versioned in 
        /// the CustomEntityVersion table.
        /// </para>
        /// <para>
        /// The CustomEntityVersion table also contains the custom data model
        /// data, which is serialized as unstructured data. Entity relations
        /// on the unstructured data model are tracked via the 
        /// UnstructuredDataDependency table.
        /// </para>
        /// </summary>
        public DbSet<CustomEntity> CustomEntities { get; set; }

        /// <summary>
        /// <para>
        /// Custom entities can have one or more version, with a collection
        /// of versions representing the change history of custom entity
        /// data. 
        /// </para>
        /// <para>
        /// Only one draft version can exist at any one time, and 
        /// only one version may be published at any one time. Although
        /// you can revert to a previous version, this is done by copying
        /// the old version data to a new version, so that a full history is
        /// always maintained.
        /// </para>
        /// <param>
        /// Typically you should query for version data via the 
        /// CustomEntityPublishStatusQuery table, which serves as a quicker
        /// look up for an applicable version for various PublishStatusQuery
        /// states.
        /// </param>
        /// </summary>
        public DbSet<CustomEntityVersion> CustomEntityVersions { get; set; }

        /// <summary>
        /// Page block data for a specific custom entity version on a custom entity
        /// details page.
        /// </summary>
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

        /// <summary>
        /// Represents a non-image file that has been uploaded to the 
        /// CMS. The name could be misleading here as any file type except
        /// images are supported, but at least it is less ambigous than the 
        /// term 'file'.
        /// </summary>
        public DbSet<DocumentAsset> DocumentAssets { get; set; }

        public DbSet<DocumentAssetTag> DocumentAssetTags { get; set; }

        public DbSet<ImageAssetGroupItem> ImageAssetGroupItems { get; set; }

        public DbSet<ImageAssetGroup> ImageAssetGroups { get; set; }

        /// <summary>
        /// Represents an image that has been uploaded to the CMS.
        /// </summary>
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

        /// <summary>
        /// A permission represents an type action a user can
        /// be permitted to perform. Typically this is associated
        /// with a specified entity type, but doesn't have to be e.g.
        /// "read pages", "access dashboard", "delete images". The 
        /// combination of EntityDefinitionCode and PermissionCode
        /// must be unique
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        public DbSet<RewriteRule> RewriteRules { get; set; }

        /// <summary>
        /// Roles are an assignable collection of permissions. Every user has to 
        /// be assigned to one role.
        /// </summary>
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
