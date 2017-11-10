using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class RoleMap : IEntityTypeConfiguration<Role>
    {
        public void Create(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role", DbConstants.CofoundrySchema);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.RoleCode)
                .IsCharType(3);

            builder.Property(s => s.UserAreaCode)
                .IsCharType(3);

            // Relationships

            // Many to many not supported yet in EF Core
            //builder.HasMany(m => m.Permissions)
            //    .WithMany(m => m.Roles)
            //    .Map(m =>
            //    {
            //        m.builder.ToTable("RolePermission");
            //        m.MapLeftKey("RoleId");
            //        m.MapRightKey("PermissionId");
            //    });

            builder.HasOne(s => s.UserArea)
                .WithMany()
                .HasForeignKey(d => d.UserAreaCode);
        }
    }
}
