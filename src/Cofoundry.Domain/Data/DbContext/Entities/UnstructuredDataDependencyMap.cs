using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofoundry.Domain.Data;

public class UnstructuredDataDependencyMap : IEntityTypeConfiguration<UnstructuredDataDependency>
{
    public void Configure(EntityTypeBuilder<UnstructuredDataDependency> builder)
    {
        builder.ToTable(nameof(UnstructuredDataDependency), DbConstants.CofoundrySchema);
        builder.HasKey(s => new { s.RootEntityDefinitionCode, s.RootEntityId, s.RelatedEntityDefinitionCode, s.RelatedEntityId });

        builder.Property(s => s.RootEntityDefinitionCode)
            .HasMaxLength(6)
            .IsRequired();

        builder.Property(s => s.RelatedEntityDefinitionCode)
            .HasMaxLength(6)
            .IsRequired();

        builder.HasOne(s => s.RootEntityDefinition)
            .WithMany()
            .HasForeignKey(d => d.RootEntityDefinitionCode);

        builder.HasOne(s => s.RelatedEntityDefinition)
            .WithMany()
            .HasForeignKey(d => d.RelatedEntityDefinitionCode);
    }
}
