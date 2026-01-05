namespace Cofoundry.Domain.Data;

/// <summary>
/// Extend DbModelBuilder to reduce boilerplate code when
/// setting up a DbContext for a Cofoundry implementation.
/// </summary>
public static class DbModelBuilderExtensions
{
    extension(ModelBuilder modelBuilder)
    {
        /// <summary>
        /// <para>
        /// Makes the "app" schema as the default. "app" is the recommended
        /// schema to use for any custom tables you add to the database, keeping
        /// them separate from Cofoundry tables and any other tables created
        /// by 3rd parties.
        /// </para>
        /// <para>
        /// This schema can also be references with DbConstants.DefaultAppSchema
        /// </para>
        /// </summary>
        /// <returns>DbModelBuilder for method chaining.</returns>
        public ModelBuilder HasAppSchema()
        {
            return modelBuilder.HasDefaultSchema(DbConstants.DefaultAppSchema);
        }

        /// <summary>
        /// Maps Cofoundry ImageAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining.</returns>
        public ModelBuilder MapCofoundryImageAssets()
        {
            modelBuilder
                .ApplyConfiguration(new ImageAssetMap())
#pragma warning disable CS0618 // Type or member is obsolete
                .ApplyConfiguration(new ImageAssetGroupMap())
                .ApplyConfiguration(new ImageAssetGroupItemMap())
#pragma warning restore CS0618 // Type or member is obsolete
                .ApplyConfiguration(new ImageAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry DocumentAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public ModelBuilder MapCofoundryDocumentAssets()
        {
            modelBuilder
                .ApplyConfiguration(new DocumentAssetMap())
#pragma warning disable CS0618 // Type or member is obsolete
                .ApplyConfiguration(new DocumentAssetGroupMap())
                .ApplyConfiguration(new DocumentAssetGroupItemMap())
#pragma warning restore CS0618 // Type or member is obsolete
                .ApplyConfiguration(new DocumentAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public ModelBuilder MapCofoundryUsers()
        {
            modelBuilder
                .ApplyConfiguration(new IPAddressMap())
                .ApplyConfiguration(new AuthorizedTaskMap())
                .ApplyConfiguration(new EmailDomainMap())
                .ApplyConfiguration(new UserMap())
                .ApplyConfiguration(new UserAuthenticationLogMap())
                .ApplyConfiguration(new UserAuthenticationFailLogMap())
                .ApplyConfiguration(new UserAreaMap())
                .ApplyConfiguration(new RoleMap())
                .ApplyConfiguration(new PermissionMap())
                .ApplyConfiguration(new RolePermissionMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public ModelBuilder MapCofoundryTags()
        {
            modelBuilder.ApplyConfiguration(new TagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry locales classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public ModelBuilder MapCofoundryLocales()
        {
            modelBuilder.ApplyConfiguration(new LocaleMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry page, custom entities, images and all dependency classes 
        /// to the DbModelBuilder.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public ModelBuilder MapCofoundryContent()
        {
            modelBuilder
                .MapCofoundryLocales()
                .MapCofoundryUsers()
                .MapCofoundryTags()
                .MapCofoundryImageAssets()
                .MapCofoundryDocumentAssets()
                .ApplyConfiguration(new AssetFileCleanupQueueItemMap())
                .ApplyConfiguration(new CustomEntityVersionPageBlockMap())
                .ApplyConfiguration(new PageTemplateMap())
                .ApplyConfiguration(new PageTemplateRegionMap())
                .ApplyConfiguration(new PageBlockTypeTemplateMap())
                .ApplyConfiguration(new PageBlockTypeMap())
#pragma warning disable CS0618 // Type or member is obsolete
                .ApplyConfiguration(new PageGroupItemMap())
                .ApplyConfiguration(new PageGroupMap())
#pragma warning restore CS0618 // Type or member is obsolete
                .ApplyConfiguration(new PageVersionBlockMap())
                .ApplyConfiguration(new PageMap())
                .ApplyConfiguration(new PageAccessRuleMap())
                .ApplyConfiguration(new PagePublishStatusQueryMap())
                .ApplyConfiguration(new PageTagMap())
                .ApplyConfiguration(new PageVersionMap())
                .ApplyConfiguration(new PageDirectoryMap())
                .ApplyConfiguration(new PageDirectoryPathMap())
                .ApplyConfiguration(new PageDirectoryClosureMap())
                .ApplyConfiguration(new PageDirectoryAccessRuleMap())
                .ApplyConfiguration(new PageDirectoryLocaleMap())
                .ApplyConfiguration(new EntityDefinitionMap())
                .ApplyConfiguration(new UnstructuredDataDependencyMap())
                .ApplyConfiguration(new CustomEntityDefinitionMap())
                .ApplyConfiguration(new CustomEntityMap())
                .ApplyConfiguration(new CustomEntityPublishStatusQueryMap())
                .ApplyConfiguration(new CustomEntityVersionMap());

            return modelBuilder;
        }
    }
}
