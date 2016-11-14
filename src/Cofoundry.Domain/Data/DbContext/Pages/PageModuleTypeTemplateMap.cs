using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class ModuleTemplateMap : EntityTypeConfiguration<PageModuleTypeTemplate>
    {
        public ModuleTemplateMap()
        {
            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships
            HasRequired(t => t.PageModuleType)
                .WithMany(t => t.PageModuleTemplates)
                .HasForeignKey(d => d.PageModuleTypeId);
        }
    }
}
