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
                .HasMaxLength(32);

            // Relationships
            HasMany(t => t.PageModuleTypes)
                .WithMany(t => t.PageTemplateSections)
                .Map(m =>
                    {
                        m.ToTable("PageTemplateSectionPageModuleType");
                        m.MapLeftKey("PageTemplateSectionId");
                        m.MapRightKey("PageModuleTypeId");
                    });

            HasRequired(t => t.PageTemplate)
                .WithMany(t => t.PageTemplateSections)
                .HasForeignKey(d => d.PageTemplateId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
