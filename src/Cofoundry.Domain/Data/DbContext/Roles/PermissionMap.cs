using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class PermissionMap : IEntityTypeConfiguration<Permission>
    {
        public void Create(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permission", DbConstants.CofoundrySchema);

            builder.Property(s => s.PermissionCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(s => s.EntityDefinitionCode)
                .HasMaxLength(6);
        }
    }
}
