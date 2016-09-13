using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionPageModuleMap : EntityTypeConfiguration<CustomEntityVersionPageModule>
    {
        public CustomEntityVersionPageModuleMap()
        {
            // Properties
            Property(t => t.SerializedData)
                .IsRequired();

            // Relationships
            HasRequired(t => t.CustomEntityVersion)
                .WithMany(t => t.CustomEntityVersionPageModules)
                .HasForeignKey(d => d.CustomEntityVersionId);

            HasRequired(t => t.PageModuleType)
                .WithMany()
                .HasForeignKey(d => d.PageModuleTypeId);

            HasRequired(t => t.PageTemplateSection)
                .WithMany()
                .HasForeignKey(d => d.PageTemplateSectionId);

            HasOptional(t => t.PageModuleTypeTemplate)
                .WithMany()
                .HasForeignKey(d => d.PageModuleTypeTemplateId);

        }
    }
}
