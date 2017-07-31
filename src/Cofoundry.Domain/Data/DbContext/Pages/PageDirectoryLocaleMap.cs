using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageDirectoryLocaleMap : IEntityTypeConfiguration<PageDirectoryLocale>
    {
        public void Create(EntityTypeBuilder<PageDirectoryLocale> builder)
        {
            builder.ToTable("PageDirectoryLocale", DbConstants.CofoundrySchema);

            // Primary Key

            builder.HasKey(s => s.PageDirectoryLocaleId);

            // Properties
            builder.Property(s => s.UrlPath)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships

            builder.HasOne(s => s.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.PageDirectory)
                .WithMany(s => s.PageDirectoryLocales)
                .HasForeignKey(d => d.PageDirectoryId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
