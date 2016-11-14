using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageTemplateSectionMap : EntityTypeConfiguration<PageTemplateSection>
    {
        public PageTemplateSectionMap()
        {
            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships

            HasRequired(t => t.PageTemplate)
                .WithMany(t => t.PageTemplateSections)
                .HasForeignKey(d => d.PageTemplateId);
        }
    }
}
