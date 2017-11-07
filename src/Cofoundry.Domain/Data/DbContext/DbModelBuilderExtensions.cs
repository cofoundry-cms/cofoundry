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
        /// Use the suggested config for your DbContext, which removes the PluralizingTableNameConvention
        /// and makes "app" the default schema.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder UseDefaultConfig(this ModelBuilder modelBuilder)
        {
            return modelBuilder.UseDefaultConfig(DbConstants.DefaultAppSchema);
        }

        /// <summary>
        /// Use the suggested config for your DbContext, which removes the PluralizingTableNameConvention
        /// and makes "app" the default schema.
        /// </summary>
        /// <param name="schema">Name of the schema to use by default.</param>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder UseDefaultConfig(this ModelBuilder modelBuilder, string schema)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema(schema);

            return modelBuilder;
        }

        /// <summary>
        /// Shortcut to modelBuilder.Configurations.Add&lt;TEntityType&gt;() which
        /// also allows for method chaining
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder Map<TEntityType>(this ModelBuilder modelBuilder, IEntityTypeConfiguration<TEntityType> entityTypeConfiguration) where TEntityType : class
        {
            modelBuilder.Entity<TEntityType>(entityTypeConfiguration.Create);

            return modelBuilder;
        }

        #region common Cofoundry object mapping

        /// <summary>
        /// Maps Cofoundry ImageAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryImageAssets(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Map(new ImageAssetMap())
                .Map(new ImageAssetGroupMap())
                .Map(new ImageAssetGroupItemMap())
                .Map(new ImageAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry DocumentAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryDocumentAssets(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Map(new DocumentAssetMap())
                .Map(new DocumentAssetGroupMap())
                .Map(new DocumentAssetGroupItemMap())
                .Map(new DocumentAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Map(new UserMap())
                .Map(new UserAreaMap())
                .Map(new RoleMap())
                .Map(new PermissionMap())
                .Map(new RolePermissionMap())
                .Map(new UserPasswordResetRequestMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryTags(this ModelBuilder modelBuilder)
        {
            modelBuilder.Map(new TagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry locales classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static ModelBuilder MapCofoundryLocales(this ModelBuilder modelBuilder)
        {
            modelBuilder.Map(new LocaleMap());

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
                .Map(new CustomEntityVersionPageBlockMap())
                .Map(new PageTemplateMap())
                .Map(new PageTemplateRegionMap())
                .Map(new PageBlockTypeTemplateMap())
                .Map(new PageBlockTypeMap())
                .Map(new PageGroupItemMap())
                .Map(new PageGroupMap())
                .Map(new PageVersionBlockMap())
                .Map(new PageMap())
                .Map(new PagePublishStatusQueryMap())
                .Map(new PageTagMap())
                .Map(new PageVersionMap())
                .Map(new PageDirectoryMap())
                .Map(new PageDirectoryLocaleMap())
                .Map(new EntityDefinitionMap())
                .Map(new UnstructuredDataDependencyMap())
                .Map(new CustomEntityDefinitionMap())
                .Map(new CustomEntityMap())
                .Map(new CustomEntityPublishStatusQueryMap())
                .Map(new CustomEntityVersionMap());

            return modelBuilder;
        }

        #endregion
    }
}
