using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core.EntityFramework;

namespace Cofoundry.Domain.Data
{
    public class PageBlockTypeMap : IEntityTypeConfiguration<PageBlockType>
    {
        public void Configure(EntityTypeBuilder<PageBlockType> builder)
        {
            builder.ToTable("PageBlockType", DbConstants.CofoundrySchema);

            // Properties

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Description)
                .IsNVarCharMaxType();

            builder.Property(s => s.FileName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
