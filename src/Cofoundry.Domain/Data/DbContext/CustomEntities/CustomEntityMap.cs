using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityMap : EntityTypeConfiguration<CustomEntity>
    {
        public CustomEntityMap()
        {
            // Properties
            Property(t => t.CustomEntityDefinitionCode)
                .HasMaxLength(6)
                .IsFixedLength()
                .IsUnicode(false);

            Property(t => t.UrlSlug)
                .HasMaxLength(200)
                .IsRequired();

            // Relationships
            HasRequired(t => t.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode);

            HasOptional(t => t.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
