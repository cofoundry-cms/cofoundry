using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionMap : IEntityTypeConfiguration<CustomEntityVersion>
    {
        public void Configure(EntityTypeBuilder<CustomEntityVersion> builder)
        {
            builder.ToTable("CustomEntityVersion", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.SerializedData)
                .IsRequired();

            builder.Property(s => s.Title)
                .HasMaxLength(200)
                .IsRequired();

            // Relationships

            builder.HasOne(s => s.CustomEntity)
                .WithMany(s => s.CustomEntityVersions)
                .HasForeignKey(d => d.CustomEntityId);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
