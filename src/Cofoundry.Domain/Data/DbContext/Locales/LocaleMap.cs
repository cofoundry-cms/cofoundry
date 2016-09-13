using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class LocaleMap : EntityTypeConfiguration<Locale>
    {
        public LocaleMap()
        {
            ToTable("Locale", DbConstants.CofoundrySchema);

            // Primary Key
            HasKey(t => t.LocaleId);

            // Properties
            Property(t => t.IETFLanguageTag)
                .IsRequired()
                .HasMaxLength(16);

            Property(t => t.LocaleName)
                .IsRequired()
                .HasMaxLength(64);
            
            // Relationships
            HasOptional(t => t.ParentLocale)
                .WithMany(t => t.ChildLocales)
                .HasForeignKey(d => d.ParentLocaleId);

        }
    }
}
