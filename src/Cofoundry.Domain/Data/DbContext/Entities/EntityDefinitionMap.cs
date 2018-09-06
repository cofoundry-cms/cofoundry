using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain.Data
{
    public class EntityDefinitionMap : IEntityTypeConfiguration<EntityDefinition>
    {
        public void Configure(EntityTypeBuilder<EntityDefinition> builder)
        {
            builder.ToTable("EntityDefinition", DbConstants.CofoundrySchema);

            builder.HasKey(s => s.EntityDefinitionCode);

            // Properties

            builder.Property(s => s.EntityDefinitionCode)
                .IsRequired()
                .IsCharType(6);

            builder.Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
