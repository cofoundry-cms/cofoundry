using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class UserPasswordResetRequestMap : IEntityTypeConfiguration<UserPasswordResetRequest>
    {
        public void Configure(EntityTypeBuilder<UserPasswordResetRequest> builder)
        {
            builder.ToTable("UserPasswordResetRequest", DbConstants.CofoundrySchema);

            builder.Property(s => s.Token)
                .IsVarCharMaxType();

            builder.Property(s => s.IPAddress)
                .IsUnicode(false)
                .HasMaxLength(45);

            // Relationships

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);
        }
    }
}
