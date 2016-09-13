using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            // Properties
            Property(t => t.SettingKey)
                .IsRequired()
                .HasMaxLength(32);

            Property(t => t.SettingValue)
                .IsRequired();
        }
    }
}
