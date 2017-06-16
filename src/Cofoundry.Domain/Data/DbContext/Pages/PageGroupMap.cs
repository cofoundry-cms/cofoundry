using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageGroupMap : IEntityTypeConfiguration<PageGroup>
    {
        public void Create(EntityTypeBuilder<PageGroup> builder)
        {
            builder.ToTable("PageGroup", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.GroupName)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships

            builder.HasOne(s => s.ParentPageGroup)
                .WithMany(s => s.ChildPageGroups)
                .HasForeignKey(d => d.ParentGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
