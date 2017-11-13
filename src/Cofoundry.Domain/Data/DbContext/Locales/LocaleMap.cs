using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class LocaleMap : IEntityTypeConfiguration<Locale>
    {
        public void Configure(EntityTypeBuilder<Locale> builder)
        {
            builder.ToTable("Locale", DbConstants.CofoundrySchema);

            // Primary Key
            builder.HasKey(s => s.LocaleId);

            // Properties

            builder.Property(s => s.IETFLanguageTag)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(s => s.LocaleName)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships

            builder.HasOne(s => s.ParentLocale)
                .WithMany(s => s.ChildLocales)
                .HasForeignKey(d => d.ParentLocaleId);
        }
    }
}
