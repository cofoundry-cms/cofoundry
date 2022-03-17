using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class IPAddressMap : IEntityTypeConfiguration<IPAddress>
{
    public void Configure(EntityTypeBuilder<IPAddress> builder)
    {
        builder.ToTable(nameof(IPAddress), DbConstants.CofoundrySchema);

        builder.Property(s => s.Address)
            .IsUnicode(false)
            .HasMaxLength(45)
            .IsRequired(true);

        builder.Property(s => s.CreateDate).IsUtc();
    }
}
