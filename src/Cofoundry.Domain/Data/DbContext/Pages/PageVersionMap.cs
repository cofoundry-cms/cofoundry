using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageVersionMap : EntityTypeConfiguration<PageVersion>
    {
        public PageVersionMap()
        {
            // Properties
            Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            Property(t => t.MetaDescription)
                .IsRequired()
                .HasMaxLength(300);

            Property(t => t.OpenGraphTitle)
                .HasMaxLength(300);

            // Relationships
            HasOptional(t => t.OpenGraphImageAsset)
                .WithMany()
                .HasForeignKey(d => d.OpenGraphImageId);
            HasRequired(t => t.PageTemplate)
                .WithMany(t => t.PageVersions)
                .HasForeignKey(d => d.PageTemplateId);
            HasRequired(t => t.Page)
                .WithMany(t => t.PageVersions)
                .HasForeignKey(d => d.PageId);
            HasOptional(t => t.BasedOnPageVersion)
                .WithMany(t => t.ChildPageVersions)
                .HasForeignKey(d => d.BasedOnPageVersionId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
