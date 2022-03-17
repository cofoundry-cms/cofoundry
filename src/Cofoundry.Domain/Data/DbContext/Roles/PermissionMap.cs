using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class PermissionMap : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(nameof(Permission), DbConstants.CofoundrySchema);

        builder.Property(s => s.PermissionCode)
            .IsRequired()
            .IsCharType(6);

        builder.Property(s => s.EntityDefinitionCode)
            .IsCharType(6);

        builder.HasOne(s => s.EntityDefinition)
            .WithMany(d => d.Permissions)
            .HasForeignKey(s => s.EntityDefinitionCode);
    }
}
