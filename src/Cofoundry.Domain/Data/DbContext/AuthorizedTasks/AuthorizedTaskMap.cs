using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class AuthorizedTaskMap : IEntityTypeConfiguration<AuthorizedTask>
    {
        public void Configure(EntityTypeBuilder<AuthorizedTask> builder)
        {
            builder.ToTable(nameof(AuthorizedTask), DbConstants.CofoundrySchema);

            builder.Property(s => s.AuthorizationCode)
                .IsVarCharMaxType()
                .IsRequired();

            builder.Property(s => s.AuthorizedTaskTypeCode)
                .IsCharType(6)
                .IsRequired();

            builder.Property(s => s.TaskData)
                .IsNVarCharMaxType();

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);

            builder.HasOne(s => s.IPAddress)
                .WithMany(d => d.AuthorizedTasks)
                .HasForeignKey(s => s.IPAddressId);

            builder.Property(s => s.CreateDate).IsUtc();
            builder.Property(s => s.InvalidatedDate).IsUtc();
            builder.Property(s => s.ExpiryDate).IsUtc();
            builder.Property(s => s.CompletedDate).IsUtc();
        }
    }
}