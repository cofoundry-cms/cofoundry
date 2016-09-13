using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class UserAreaMap : EntityTypeConfiguration<UserArea>
    {
        public UserAreaMap()
        {
            ToTable("UserArea", DbConstants.CofoundrySchema);

            HasKey(t => t.UserAreaCode);

            Property(t => t.UserAreaCode)
                .HasMaxLength(3)
                .IsFixedLength()
                .IsUnicode(false);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}
