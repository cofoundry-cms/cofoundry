using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class CustomEntityVersionMap : IEntityTypeConfiguration<CustomEntityVersion>
{
    public void Configure(EntityTypeBuilder<CustomEntityVersion> builder)
    {
        builder.ToTable(nameof(CustomEntityVersion), DbConstants.CofoundrySchema);

        builder.Property(s => s.SerializedData)
            .IsRequired();

        builder.Property(s => s.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(s => s.CustomEntity)
            .WithMany(s => s.CustomEntityVersions)
            .HasForeignKey(d => d.CustomEntityId);

        CreateAuditableMappingHelper.Map(builder);
    }
}
