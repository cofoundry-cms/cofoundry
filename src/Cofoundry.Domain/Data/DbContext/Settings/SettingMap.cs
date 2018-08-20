using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain.Data
{
    public class SettingMap : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.ToTable("Setting", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.SettingKey)
                .IsRequired()
                .HasMaxLength(32);

            builder.Property(s => s.SettingValue)
                .IsRequired()
                .IsNVarCharMaxType();
        }
    }
}
