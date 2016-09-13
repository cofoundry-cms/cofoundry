using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionMap : EntityTypeConfiguration<CustomEntityVersion>
    {
        public CustomEntityVersionMap()
        {
            // Properties
            Property(t => t.SerializedData)
                .IsRequired();

            Property(t => t.Title)
                .HasMaxLength(200)
                .IsRequired();

            // Relationships
            HasRequired(t => t.WorkFlowStatus)
                .WithMany()
                .HasForeignKey(d => d.WorkFlowStatusId);

            HasRequired(t => t.CustomEntity)
                .WithMany(t => t.CustomEntityVersions)
                .HasForeignKey(d => d.CustomEntityId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
