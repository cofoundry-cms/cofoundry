using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageTagMap : EntityTypeConfiguration<PageTag>
    {
        public PageTagMap()
        {
            // Primary Key
            HasKey(t => new { t.PageId, t.TagId });

            // Properties
            Property(t => t.PageId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.TagId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Relationships
            HasRequired(t => t.Page)
                .WithMany(t => t.PageTags)
                .HasForeignKey(d => d.PageId);
            HasRequired(t => t.Tag)
                .WithMany()
                .HasForeignKey(d => d.TagId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
