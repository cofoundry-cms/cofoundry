using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityDefinitionMap : IEntityTypeConfiguration<CustomEntityDefinition>
    {
        public void Configure(EntityTypeBuilder<CustomEntityDefinition> builder)
        {
            builder.ToTable(nameof(CustomEntityDefinition), DbConstants.CofoundrySchema);
            builder.HasKey(s => s.CustomEntityDefinitionCode);

            builder.Property(s => s.CustomEntityDefinitionCode)
                .IsRequired()
                .IsCharType(6);

            builder.HasOne(s => s.EntityDefinition)
                .WithOne()
                .HasForeignKey<CustomEntityDefinition>(s => s.CustomEntityDefinitionCode);
        }
    }
}