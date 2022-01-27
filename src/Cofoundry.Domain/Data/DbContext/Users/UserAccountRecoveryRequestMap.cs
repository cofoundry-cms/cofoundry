using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class UserAccountRecoveryRequestMap : IEntityTypeConfiguration<UserAccountRecoveryRequest>
    {
        public void Configure(EntityTypeBuilder<UserAccountRecoveryRequest> builder)
        {
            builder.ToTable(nameof(UserAccountRecoveryRequest), DbConstants.CofoundrySchema);

            builder.Property(s => s.AuthorizationCode)
                .IsVarCharMaxType();

            builder.Property(s => s.IPAddress)
                .IsUnicode(false)
                .HasMaxLength(45);

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);

            builder.Property(s => s.CreateDate).IsUtc();
            builder.Property(s => s.InvalidatedDate).IsUtc();
            builder.Property(s => s.CompletedDate).IsUtc();
        }
    }
}