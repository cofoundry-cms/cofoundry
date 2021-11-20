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
    public class CatLikeCountMap : IEntityTypeConfiguration<CatLikeCount>
    {
        public void Configure(EntityTypeBuilder<CatLikeCount> builder)
        {
            builder.ToTable("CatLikeCount", DbConstants.DefaultAppSchema);

            builder.HasKey(e => e.CatCustomEntityId);
        }
    }
}
