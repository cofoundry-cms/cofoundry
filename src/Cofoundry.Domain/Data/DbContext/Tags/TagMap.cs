using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class TagMap : IEntityTypeConfiguration<Tag>
    {
        public void Create(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tag", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.TagText)
                .IsRequired()
                .HasMaxLength(32);
        }
    }
}
