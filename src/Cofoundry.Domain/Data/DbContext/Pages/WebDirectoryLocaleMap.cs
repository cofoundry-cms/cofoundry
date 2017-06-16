using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class WebDirectoryLocaleMap : IEntityTypeConfiguration<WebDirectoryLocale>
    {
        public void Create(EntityTypeBuilder<WebDirectoryLocale> builder)
        {
            builder.ToTable("WebDirectoryLocale", DbConstants.CofoundrySchema);

            // Primary Key

            builder.HasKey(s => s.WebDirectoryLocaleId);

            // Properties
            builder.Property(s => s.UrlPath)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships

            builder.HasOne(s => s.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.WebDirectory)
                .WithMany(s => s.WebDirectoryLocales)
                .HasForeignKey(d => d.WebDirectoryId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
