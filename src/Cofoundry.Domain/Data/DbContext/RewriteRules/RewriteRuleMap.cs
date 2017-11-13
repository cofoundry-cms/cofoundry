using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Domain.Data
{
    public class RewriteRuleMap : IEntityTypeConfiguration<RewriteRule>
    {
        public void Configure(EntityTypeBuilder<RewriteRule> builder)
        {
            builder.ToTable("RewriteRule", DbConstants.CofoundrySchema);

            // Properties
            builder.Property(s => s.WriteFrom)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(s => s.WriteTo)
                .IsRequired()
                .HasMaxLength(2000);

            CreateAuditableMappingHelper.Map(builder);
        }
    }
}
