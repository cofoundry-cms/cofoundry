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
            modelBuilder.Map(new ImageAssetMap());
            modelBuilder.Map(new ImageAssetGroupMap());
            modelBuilder.Map(new ImageAssetGroupItemMap());
            modelBuilder.Map(new ImageAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry DocumentAsset classes to the DbModelBuilder. Requires Cofoundry Users and Tags to be mapped.
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryDocumentAssets(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Map(new DocumentAssetMap());
            modelBuilder.Map(new DocumentAssetGroupMap());
            modelBuilder.Map(new DocumentAssetGroupItemMap());
            modelBuilder.Map(new DocumentAssetTagMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry Users classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryUsers(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Map(new UserMap());
            modelBuilder.Map(new UserAreaMap());
            modelBuilder.Map(new RoleMap());
            modelBuilder.Map(new PermissionMap());

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
        /// Maps Cofoundry entity definition and unscructured dependency classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryEntityDefinitions(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Map(new EntityDefinitionMap());
            modelBuilder.Map(new UnstructuredDataDependencyMap());

            return modelBuilder;
        }

        /// <summary>
        /// Maps Cofoundry custom entity classes to the DbModelBuilder
        /// </summary>
        /// <returns>DbModelBuilder for method chaining</returns>
        public static DbModelBuilder MapCofoundryCustomEntities(this DbModelBuilder modelBuilder)
        {
            modelBuilder.Map(new CustomEntityDefinitionMap());
            modelBuilder.Map(new CustomEntityMap());
            modelBuilder.Map(new CustomEntityVersionMap());

            return modelBuilder;
        }

        #endregion
    }
}
