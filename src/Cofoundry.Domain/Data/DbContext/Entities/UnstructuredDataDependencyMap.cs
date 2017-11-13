using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class UnstructuredDataDependencyMap : IEntityTypeConfiguration<UnstructuredDataDependency>
    {
        public void Configure(EntityTypeBuilder<UnstructuredDataDependency> builder)
        {
            builder.ToTable("UnstructuredDataDependency", DbConstants.CofoundrySchema);

            builder.HasKey(s => new { s.RootEntityDefinitionCode, s.RootEntityId, s.RelatedEntityDefinitionCode, s.RelatedEntityId });

            // Properties

            builder.Property(s => s.RootEntityDefinitionCode)
                .HasMaxLength(6)
                .IsRequired();

            builder.Property(s => s.RelatedEntityDefinitionCode)
                .HasMaxLength(6)
                .IsRequired();

            // Relations

            builder.HasOne(s => s.RootEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.RootEntityDefinitionCode);

            // Relations

            builder.HasOne(s => s.RelatedEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.RelatedEntityDefinitionCode);
        }
    }
}
