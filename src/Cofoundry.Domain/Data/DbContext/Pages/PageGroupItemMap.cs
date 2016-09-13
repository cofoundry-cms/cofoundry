using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageGroupItemMap : EntityTypeConfiguration<PageGroupItem>
    {
        public PageGroupItemMap()
        {
            // Primary Key
            HasKey(t => new { t.PageId, t.PageGroupId });

            // Properties
            Property(t => t.PageId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.PageGroupId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings

            // Relationships
            HasRequired(t => t.PageGroup)
                .WithMany(t => t.PageGroupItems)
                .HasForeignKey(d => d.PageGroupId);
            HasRequired(t => t.Page)
                .WithMany(t => t.PageGroupItems)
                .HasForeignKey(d => d.PageId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
