using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityMap : IEntityTypeConfiguration<CustomEntity>
    {
        public void Create(EntityTypeBuilder<CustomEntity> builder)
        {
            builder.ToTable("CustomEntity", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.CustomEntityDefinitionCode)
                .IsRequired()
                .IsCharType(6);

            builder.Property(s => s.UrlSlug)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.PublishStatusCode)
                .HasMaxLength(1)
                .IsUnicode(false);

            // Relationships

            builder.HasOne(s => s.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId)
                .OnDelete(DeleteBehavior.Restrict);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
