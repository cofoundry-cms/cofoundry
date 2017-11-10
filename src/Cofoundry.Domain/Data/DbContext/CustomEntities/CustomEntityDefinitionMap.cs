using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityDefinitionMap : IEntityTypeConfiguration<CustomEntityDefinition>
    {
        public void Create(EntityTypeBuilder<CustomEntityDefinition> builder)
        {
            builder.ToTable("CustomEntityDefinition", DbConstants.CofoundrySchema);

            builder.HasKey(s => s.CustomEntityDefinitionCode);

            // Properties

            builder.Property(s => s.CustomEntityDefinitionCode)
                .IsRequired()
                .IsCharType(6);

            // Relations

            builder.HasOne(s => s.EntityDefinition)
                .WithOne()
                .HasForeignKey<CustomEntityDefinition>(s => s.CustomEntityDefinitionCode);
        }
    }
}
