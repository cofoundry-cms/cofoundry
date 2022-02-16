using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User), DbConstants.CofoundrySchema);

            builder.Property(s => s.FirstName)
                .HasMaxLength(32);

            builder.Property(s => s.LastName)
                .HasMaxLength(32);

            builder.Property(s => s.Email)
                .HasMaxLength(150);

            builder.Property(s => s.UniqueEmail)
                .HasMaxLength(150);

            builder.Property(s => s.Username)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.UniqueUsername)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.Password)
                .IsNVarCharMaxType();

            builder.Property(s => s.UserAreaCode)
                .IsRequired()
                .IsCharType(3);

            builder.Property(s => s.SecurityStamp)
                .IsRequired()
                .IsNVarCharMaxType();

            builder.Property(s => s.LastSignInDate).IsUtc();
            builder.Property(s => s.LastPasswordChangeDate).IsUtc();
            builder.Property(s => s.PreviousSignInDate).IsUtc();
            builder.Property(s => s.CreateDate).IsUtc();
            builder.Property(s => s.DeactivatedDate).IsUtc();
            builder.Property(s => s.DeletedDate).IsUtc();

            builder.HasOne(s => s.EmailDomain)
                .WithMany(d => d.Users)
                .HasForeignKey(d => d.EmailDomainId);

            builder.HasOne(s => s.Role)
                .WithMany()
                .HasForeignKey(d => d.RoleId);

            builder.HasOne(s => s.Creator)
                .WithMany()
                .HasForeignKey(d => d.CreatorId);

            builder.HasOne(s => s.UserArea)
                .WithMany(d => d.Users)
                .HasForeignKey(d => d.UserAreaCode);
        }
    }
}