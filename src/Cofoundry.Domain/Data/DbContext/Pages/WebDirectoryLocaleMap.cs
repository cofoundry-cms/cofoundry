using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class WebDirectoryLocaleMap : EntityTypeConfiguration<WebDirectoryLocale>
    {
        public WebDirectoryLocaleMap()
        {
            // Primary Key
            HasKey(t => t.WebDirectoryLocaleId);

            // Properties
            Property(t => t.UrlPath)
                .IsRequired()
                .HasMaxLength(64);

            // Relationships
            HasRequired(t => t.Locale)
                .WithMany()
                .HasForeignKey(d => d.LocaleId);
            HasRequired(t => t.WebDirectory)
                .WithMany(t => t.WebDirectoryLocales)
                .HasForeignKey(d => d.WebDirectoryId);

            CreateAuditableMappingHelper.Map(this);
        }
    }
}
