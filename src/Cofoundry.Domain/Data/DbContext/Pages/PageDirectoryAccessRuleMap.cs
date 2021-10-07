using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageDirectoryAccessRuleMap : IEntityTypeConfiguration<PageDirectoryAccessRule>
    {
        public void Configure(EntityTypeBuilder<PageDirectoryAccessRule> builder)
        {
            builder.ToTable(nameof(PageDirectoryAccessRule), DbConstants.CofoundrySchema);

            builder.Property(s => s.UserAreaCode)
                .IsRequired()
                .IsCharType(3);

            builder
                .HasOne(s => s.Creator)
                .WithMany()
                .HasForeignKey(s => s.CreatorId);

            builder
                .HasOne(s => s.PageDirectory)
                .WithMany(d => d.PageDirectoryAccessRules)
                .HasForeignKey(s => s.PageDirectoryId);

            builder
                .HasOne(s => s.Role)
                .WithMany(d => d.PageDirectoryAccessRules)
                .HasForeignKey(s => s.RoleId);

            builder
                .HasOne(s => s.UserArea)
                .WithMany(d => d.PageDirectoryAccessRules)
                .HasForeignKey(s => s.UserAreaCode);
        }
    }
}
