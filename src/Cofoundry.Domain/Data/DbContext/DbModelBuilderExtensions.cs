using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Extend DbModelBuilder to reduce boilerplate code when
    /// setting up a DbContext for a Cofoundry implementation.
    /// </summary>
    public static class DbModelBuilderExtensions
    {
        /// <summary>
        /// Use the suggested config for your DbContext, which currenly only 
        /// makes "app" the default schema.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining.</returns>
        [Obsolete("Replaced with HasAppSchema() because no other configuration takes place here.")]
        public static ModelBuilder UseDefaultConfig(this ModelBuilder modelBuilder)
        {
            return modelBuilder.HasAppSchema();
        }

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
        public static ModelBuilder HasAppSchema(this ModelBuilder modelBuilder)
        {
            return modelBuilder.HasDefaultSchema(DbConstants.DefaultAppSchema);
        }

        #region common Cofoundry object mapping

        /// <summary>
        /// Maps Cofoundry ImageAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining.</returns>
        public static ModelBuilder MapCofoundryImageAssets(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new ImageAssetMap())
                .ApplyConfiguration(new ImageAssetGroupMap())
                .ApplyConfiguration(new ImageAssetGroupItemMap())
                .ApplyConfiguration(new ImageAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry DocumentAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryDocumentAssets(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new DocumentAssetMap())
                .ApplyConfiguration(new DocumentAssetGroupMap())
                .ApplyConfiguration(new DocumentAssetGroupItemMap())
                .ApplyConfiguration(new DocumentAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new UserMap())
                .ApplyConfiguration(new UserAreaMap())
                .ApplyConfiguration(new RoleMap())
                .ApplyConfiguration(new PermissionMap())
                .ApplyConfiguration(new RolePermissionMap())
                .ApplyConfiguration(new UserPasswordResetRequestMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryTags(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry locales classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryLocales(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LocaleMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry page, custom entities, images and all dependency classes 
        /// to the DbModelBuilder.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryContent(this ModelBuilder modelBuilder)
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
                .ApplyConfiguration(new PageGroupItemMap())
                .ApplyConfiguration(new PageGroupMap())
                .ApplyConfiguration(new PageVersionBlockMap())
                .ApplyConfiguration(new PageMap())
                .ApplyConfiguration(new PagePublishStatusQueryMap())
                .ApplyConfiguration(new PageTagMap())
                .ApplyConfiguration(new PageVersionMap())
                .ApplyConfiguration(new PageDirectoryMap())
                .ApplyConfiguration(new PageDirectoryLocaleMap())
                .ApplyConfiguration(new EntityDefinitionMap())
                .ApplyConfiguration(new UnstructuredDataDependencyMap())
                .ApplyConfiguration(new CustomEntityDefinitionMap())
                .ApplyConfiguration(new CustomEntityMap())
                .ApplyConfiguration(new CustomEntityPublishStatusQueryMap())
                .ApplyConfiguration(new CustomEntityVersionMap())
                ;

            return modelBuilder;
        }

        #endregion
    }
}
