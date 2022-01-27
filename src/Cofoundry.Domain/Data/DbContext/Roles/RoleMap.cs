using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class RoleMap : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(nameof(Role), DbConstants.CofoundrySchema);

            builder.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.RoleCode)
                .IsCharType(3);

            builder.Property(s => s.UserAreaCode)
                .IsCharType(3);

            builder.HasOne(s => s.UserArea)
                .WithMany()
                .HasForeignKey(s => s.UserAreaCode);
        }
    }
}