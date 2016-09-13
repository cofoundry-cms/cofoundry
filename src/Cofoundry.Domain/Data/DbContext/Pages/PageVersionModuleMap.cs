using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageVersionModuleMap : EntityTypeConfiguration<PageVersionModule>
    {
        public PageVersionModuleMap()
        {
            // Properties
            Property(t => t.SerializedData)
                .IsRequired();

            // Relationships
            HasRequired(t => t.PageTemplateSection)
                .WithMany(t => t.PageVersionModules)
                .HasForeignKey(d => d.PageTemplateSectionId);
            HasRequired(t => t.PageModuleType)
                .WithMany(t => t.PageVersionModules)
                .HasForeignKey(d => d.PageModuleTypeId);
            HasRequired(t => t.PageVersion)
                .WithMany(t => t.PageVersionModules)
                .HasForeignKey(d => d.PageVersionId);
            HasOptional(t => t.PageModuleTypeTemplate)
                .WithMany()
                .HasForeignKey(t => t.PageModuleTypeTemplateId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
