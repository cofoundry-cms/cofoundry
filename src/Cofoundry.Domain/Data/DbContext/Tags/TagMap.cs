using Cofoundry.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Cofoundry.Domain.Data
{
    public class TagMap : EntityTypeConfiguration<Tag>
    {
        public TagMap()
        {
            ToTable("Tag", DbConstants.CofoundrySchema);

            // Properties
            Property(t => t.TagText)
                .IsRequired()
                .HasMaxLength(32);
        }
    }
}
