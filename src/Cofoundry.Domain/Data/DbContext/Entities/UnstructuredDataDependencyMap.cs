using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class UnstructuredDataDependencyMap : EntityTypeConfiguration<UnstructuredDataDependency>
    {
        public UnstructuredDataDependencyMap()
        {
            HasKey(t => new { t.RootEntityDefinitionCode, t.RootEntityId, t.RelatedEntityDefinitionCode, t.RelatedEntityId });

            // Properties

            Property(t => t.RootEntityDefinitionCode)
                .HasMaxLength(6)
                .IsRequired();

            Property(t => t.RelatedEntityDefinitionCode)
                .HasMaxLength(6)
                .IsRequired();

            // Relations

            HasRequired(t => t.RootEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.RootEntityDefinitionCode);

            // Relations

            HasRequired(t => t.RelatedEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.RelatedEntityDefinitionCode);


        }
    }
}
