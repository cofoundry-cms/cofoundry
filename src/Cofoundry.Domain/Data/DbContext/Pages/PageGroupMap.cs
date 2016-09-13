using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageGroupMap : EntityTypeConfiguration<PageGroup>
    {
        public PageGroupMap()
        {
            // Properties
            Property(t => t.GroupName)
                .IsRequired()
                .HasMaxLength(64);
            
            // Relationships
            HasOptional(t => t.ParentPageGroup)
                .WithMany(t => t.ChildPageGroups)
                .HasForeignKey(d => d.ParentGroupId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
