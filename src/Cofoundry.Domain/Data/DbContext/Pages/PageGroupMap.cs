using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageGroupMap : IEntityTypeConfiguration<PageGroup>
    {
        public void Configure(EntityTypeBuilder<PageGroup> builder)
        {
            builder.ToTable("PageGroup", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.GroupName)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships

            builder.HasOne(s => s.ParentPageGroup)
                .WithMany(s => s.ChildPageGroups)
                .HasForeignKey(d => d.ParentGroupId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
