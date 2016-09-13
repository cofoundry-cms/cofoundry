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
                .HasMaxLength(32);

            Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(32);

            // Relationships
            HasRequired(t => t.PageModuleType)
                .WithMany(t => t.PageModuleTemplates)
                .HasForeignKey(d => d.PageModuleTypeId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
