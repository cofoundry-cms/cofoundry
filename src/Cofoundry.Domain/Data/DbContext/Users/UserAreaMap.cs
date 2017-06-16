using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cofoundry.Domain.Data
{
    public class UserAreaMap : IEntityTypeConfiguration<UserArea>
    {
        public void Create(EntityTypeBuilder<UserArea> builder)
        {
            builder.ToTable("UserArea", DbConstants.CofoundrySchema);

            builder.HasKey(s => s.UserAreaCode);

            builder.Property(s => s.UserAreaCode)
                .IsRequired()
                .IsCharType(3);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}
