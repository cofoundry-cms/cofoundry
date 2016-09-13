using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class UpdateAuditableMappingHelper
    {
        public static void Map<T>(EntityTypeConfiguration<T> entity) where T : class, IUpdateAuditable
        {
            CreateAuditableMappingHelper.Map(entity);

            // Relationships

            entity.HasRequired(t => t.Updater)
                .WithMany()
                .HasForeignKey(d => d.UpdaterId);
        }
    }
}
