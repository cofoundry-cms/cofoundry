using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class SettingMap : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable(nameof(Setting), DbConstants.CofoundrySchema);

        builder.Property(s => s.SettingKey)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(s => s.SettingValue)
            .IsRequired()
            .IsNVarCharMaxType();

        builder.Property(s => s.CreateDate).IsUtc();
        builder.Property(s => s.UpdateDate).IsUtc();
    }
}
