using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class EntityDefinitionMap : EntityTypeConfiguration<EntityDefinition>
    {
        public EntityDefinitionMap()
        {
            HasKey(t => t.EntityDefinitionCode);

            // Properties

            Property(t => t.EntityDefinitionCode)
                .HasMaxLength(6)
                .IsFixedLength()
                .IsUnicode(false);

            Property(t => t.Name)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
