using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class WebDirectoryMap : IEntityTypeConfiguration<WebDirectory>
    {
        public void Create(EntityTypeBuilder<WebDirectory> builder)
        {
            builder.ToTable("WebDirectory", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => s.WebDirectoryId);

            // Properties
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(s => s.UrlPath)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            builder.HasOne(s => s.ParentWebDirectory)
                .WithMany(s => s.ChildWebDirectories)
                .HasForeignKey(d => d.ParentWebDirectoryId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
