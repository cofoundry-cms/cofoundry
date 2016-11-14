using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageTemplateMap : EntityTypeConfiguration<PageTemplate>
    {
        public PageTemplateMap()
        {
            // Properties
            Property(t => t.FileName)
                .HasMaxLength(100)
                .IsRequired();
            Property(t => t.Name)
                .HasMaxLength(100)
                .IsRequired();
            Property(t => t.FullPath)
                .HasMaxLength(400)
                .IsRequired();
            Property(t => t.CustomEntityDefinitionCode)
                .HasMaxLength(6);
            Property(t => t.CustomEntityModelType)
                .HasMaxLength(100);

            Property(t => t.Description)
                .IsRequired();

            // Relationships
            HasOptional(t => t.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode);
        }
    }
}
