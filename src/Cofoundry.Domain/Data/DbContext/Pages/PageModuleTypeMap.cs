using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class PageModuleTypeMap : EntityTypeConfiguration<PageModuleType>
    {
        public PageModuleTypeMap()
        {
            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Description)
                .IsRequired();

            Property(t => t.FileName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
