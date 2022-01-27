using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class EntityDefinitionMap : IEntityTypeConfiguration<EntityDefinition>
    {
        public void Configure(EntityTypeBuilder<EntityDefinition> builder)
        {
            builder.ToTable(nameof(EntityDefinition), DbConstants.CofoundrySchema);
            builder.HasKey(s => s.EntityDefinitionCode);

            builder.Property(s => s.EntityDefinitionCode)
                .IsRequired()
                .IsCharType(6);

            builder.Property(s => s.Name)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}