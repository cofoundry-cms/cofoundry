using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public static class CreateAuditableMappingHelper
    {
        public static void Map<T>(EntityTypeConfiguration<T> entity) where T : class, ICreateAuditable
        {
            // Relationships
            entity.HasRequired(t => t.Creator)
                .WithMany()
                .HasForeignKey(d => d.CreatorId);
        }
    }
}
