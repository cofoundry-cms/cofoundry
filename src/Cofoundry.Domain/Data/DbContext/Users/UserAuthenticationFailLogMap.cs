using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class UserAuthenticationFailLogMap : IEntityTypeConfiguration<UserAuthenticationFailLog>
{
    public void Configure(EntityTypeBuilder<UserAuthenticationFailLog> builder)
    {
        builder.ToTable(nameof(UserAuthenticationFailLog), DbConstants.CofoundrySchema);

        builder.Property(s => s.UserAreaCode)
            .IsRequired()
            .IsCharType(3);

        builder.Property(s => s.Username)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasOne(s => s.UserArea)
            .WithMany()
            .HasForeignKey(d => d.UserAreaCode);

        builder.HasOne(s => s.IPAddress)
            .WithMany()
            .HasForeignKey(d => d.IPAddressId);
    }
}
