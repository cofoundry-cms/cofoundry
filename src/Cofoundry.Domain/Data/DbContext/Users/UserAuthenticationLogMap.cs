using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class UserAuthenticationLogMap : IEntityTypeConfiguration<UserAuthenticationLog>
    {
        public void Configure(EntityTypeBuilder<UserAuthenticationLog> builder)
        {
            builder.ToTable(nameof(UserAuthenticationLog), DbConstants.CofoundrySchema);

            builder.HasOne(s => s.IPAddress)
                .WithMany()
                .HasForeignKey(d => d.IPAddressId);

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);
        }
    }
}