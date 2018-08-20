using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityDefinitionMap : IEntityTypeConfiguration<CustomEntityDefinition>
    {
        public void Configure(EntityTypeBuilder<CustomEntityDefinition> builder)
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
