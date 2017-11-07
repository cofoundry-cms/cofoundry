using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageMap : IEntityTypeConfiguration<Page>
    {
        public void Create(EntityTypeBuilder<Page> builder)
        {
            builder.ToTable("Page", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.UrlPath)
                .IsRequired()
                .HasMaxLength(70);

            builder.Property(s => s.CustomEntityDefinitionCode)
                .HasMaxLength(6)
                .IsUnicode(false);

            builder.Property(s => s.PublishStatusCode)
                .HasMaxLength(1)
                .IsUnicode(false);

            // Relationships

            builder.HasOne(s => s.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageDirectory)
                .WithMany(s => s.Pages)
                .HasForeignKey(d => d.PageDirectoryId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
