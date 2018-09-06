using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class UpdateAuditableMappingHelper
    {
        public static void Map<T>(EntityTypeBuilder<T> builder) where T : class, IUpdateAuditable
        {
            CreateAuditableMappingHelper.Map(builder);

            // Relationships

            builder.HasOne(s => s.Updater)
                .WithMany()
                .HasForeignKey(d => d.UpdaterId);
        }
    }
}
