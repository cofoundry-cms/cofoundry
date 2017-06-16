using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cofoundry.Core;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Data
{
    public class PageModuleTypeMap : IEntityTypeConfiguration<PageModuleType>
    {
        public void Create(EntityTypeBuilder<PageModuleType> builder)
        {
            builder.ToTable("PageModuleType", DbConstants.CofoundrySchema);

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
