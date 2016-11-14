using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionPageModuleMap : EntityTypeConfiguration<CustomEntityVersionPageModule>
    {
        public CustomEntityVersionPageModuleMap()
        {
            // Properties
            Property(s => s.SerializedData)
                .IsRequired();

            // Relationships
            HasRequired(s => s.CustomEntityVersion)
                .WithMany(d => d.CustomEntityVersionPageModules)
                .HasForeignKey(s => s.CustomEntityVersionId);

            HasRequired(s => s.PageModuleType)
                .WithMany(d => d.CustomEntityVersionPageModules)
                .HasForeignKey(s => s.PageModuleTypeId);

            HasRequired(s => s.PageTemplateSection)
                .WithMany()
                .HasForeignKey(s => s.PageTemplateSectionId);

            HasOptional(s => s.PageModuleTypeTemplate)
                .WithMany()
                .HasForeignKey(s => s.PageModuleTypeTemplateId);

        }
    }
}
