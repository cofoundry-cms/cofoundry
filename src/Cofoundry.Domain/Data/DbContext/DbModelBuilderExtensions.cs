using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
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
        public static DbModelBuilder UseDefaultConfig(this DbModelBuilder modelBuilder)
        {
            return modelBuilder.UseDefaultConfig(DbConstants.DefaultAppSchema);
        }

        /// <summary>
        /// Use the suggested config for your DbContext, which removes the PluralizingTableNameConvention
        /// and makes "app" the default schema.
        /// </summary>
        /// <param name="schema">Name of the schema to use by default.</param>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder UseDefaultConfig(this DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.HasDefaultSchema(schema);

            return modelBuilder;
        }

        /// <summary>
        /// Shortcut to modelBuilder.Configurations.Add&lt;TEntityType&gt;() which
        /// also allows for method chaining
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder Map<TEntityType>(this DbModelBuilder modelBuilder, EntityTypeConfiguration<TEntityType> entityTypeConfiguration) where TEntityType : class
        {
            modelBuilder.Configurations.Add<TEntityType>(entityTypeConfiguration);

            return modelBuilder;
        }

        #region common Cofoundry object mapping

        /// <summary>
        /// Maps Cofoundry ImageAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryImageAssets(this DbModelBuilder modelBuilder)
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
        public static DbModelBuilder MapCofoundryDocumentAssets(this DbModelBuilder modelBuilder)
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
        public static DbModelBuilder MapCofoundryUsers(this DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Map(new UserMap())
                .Map(new UserAreaMap())
                .Map(new RoleMap())
                .Map(new PermissionMap())
                .Map(new UserPasswordResetRequestMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryTags(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Map(new TagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry locales classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryLocales(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Map(new LocaleMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry page, custom entities, images and all dependency classes 
        /// to the DbModelBuilder.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryContent(this DbModelBuilder modelBuilder)
        {
            modelBuilder
                .MapCofoundryLocales()
                .MapCofoundryUsers()
                .MapCofoundryTags()
                .MapCofoundryImageAssets()
                .MapCofoundryDocumentAssets()
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
                .Map(new WebDirectoryMap())
                .Map(new WebDirectoryLocaleMap())
                .Map(new EntityDefinitionMap())
                .Map(new UnstructuredDataDependencyMap())
                .Map(new CustomEntityDefinitionMap())
                .Map(new CustomEntityMap())
                .Map(new CustomEntityVersionMap());

            return modelBuilder;
        }

        #endregion
    }
}
