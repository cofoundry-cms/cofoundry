using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PermissionMap : EntityTypeConfiguration<Permission>
    {
        public PermissionMap()
        {
            ToTable("Permission", DbConstants.CofoundrySchema);

            Property(t => t.PermissionCode)
                .IsRequired()
                .HasMaxLength(6);

            Property(t => t.EntityDefinitionCode)
                .HasMaxLength(6);
        }
    }
}
