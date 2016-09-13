using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityDefinitionMap : EntityTypeConfiguration<CustomEntityDefinition>
    {
        public CustomEntityDefinitionMap()
        {
            HasKey(t => t.CustomEntityDefinitionCode);

            // Properties

            Property(t => t.CustomEntityDefinitionCode)
                .HasMaxLength(6)
                .IsFixedLength()
                .IsUnicode(false);

            // Relations

            HasRequired(t => t.EntityDefinition)
                .WithOptional();
        }
    }
}
