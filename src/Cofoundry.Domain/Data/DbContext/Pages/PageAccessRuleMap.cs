using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class PageAccessRuleMap : IEntityTypeConfiguration<PageAccessRule>
    {
        public void Configure(EntityTypeBuilder<PageAccessRule> builder)
        {
            builder.ToTable(nameof(PageAccessRule), DbConstants.CofoundrySchema);

            builder.Property(s => s.UserAreaCode)
                .IsRequired()
                .IsCharType(3);

            builder
                .HasOne(s => s.Page)
                .WithMany(d => d.AccessRules)
                .HasForeignKey(s => s.PageId);

            builder
                .HasOne(s => s.Role)
                .WithMany(d => d.PageAccessRules)
                .HasForeignKey(s => s.RoleId);

            builder
                .HasOne(s => s.UserArea)
                .WithMany(d => d.PageAccessRules)
                .HasForeignKey(s => s.UserAreaCode);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}