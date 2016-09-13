using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            ToTable("Role", DbConstants.CofoundrySchema);

            Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.SpecialistRoleTypeCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .IsUnicode(false);

            Property(t => t.UserAreaCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .IsUnicode(false);

            // Relationships
            HasMany(m => m.Permissions)
                .WithMany(m => m.Roles)
                .Map(m =>
                {
                    m.ToTable("RolePermission");
                    m.MapLeftKey("RoleId");
                    m.MapRightKey("PermissionId");
                });

            HasRequired(s => s.UserArea)
                .WithMany()
                .HasForeignKey(d => d.UserAreaCode);
        }
    }
}
