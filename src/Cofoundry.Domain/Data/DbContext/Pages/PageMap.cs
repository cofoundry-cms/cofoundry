using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            // Properties
            Property(t => t.UrlPath)
                .IsRequired()
                .HasMaxLength(70);
            Property(t => t.CustomEntityDefinitionCode)
                .HasMaxLength(6);

            // Relationships
            HasOptional(t => t.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId);

            HasOptional(t => t.CustomEntityDefinition)
                .WithMany()
                .HasForeignKey(d => d.CustomEntityDefinitionCode);

            HasRequired(t => t.WebDirectory)
                .WithMany(t => t.Pages)
                .HasForeignKey(d => d.WebDirectoryId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
