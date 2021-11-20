using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Core;

namespace Cofoundry.Samples.SPASite.Data
{
    public class CatLikeMap : IEntityTypeConfiguration<CatLike>
    {
        public void Configure(EntityTypeBuilder<CatLike> builder)
        {
            builder.ToTable("CatLike", DbConstants.DefaultAppSchema);

            builder.HasKey(e => new { e.CatCustomEntityId, e.UserId });

            // Relationships
            builder.HasOne(s => s.CatCustomEntity)
                .WithMany()
                .HasForeignKey(d => d.CatCustomEntityId);

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);
        }
    }
}
