using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class RolePermissionMap : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermission", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => new { s.RoleId, s.PermissionId });

            // Properties
            builder.Property(s => s.RoleId)
                .ValueGeneratedNever();

            builder.Property(s => s.PermissionId)
                .ValueGeneratedNever();

            // Relationships

            builder.HasOne(s => s.Role)
                .WithMany(d => d.RolePermissions)
                .HasForeignKey(s => s.RoleId);

            builder.HasOne(s => s.Permission)
                .WithMany(d => d.RolePermissions)
                .HasForeignKey(s => s.PermissionId);
        }
    }
}
