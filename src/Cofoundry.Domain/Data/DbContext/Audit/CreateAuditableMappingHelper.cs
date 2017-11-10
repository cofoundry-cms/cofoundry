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
    public static class CreateAuditableMappingHelper
    {
        public static void Map<T>(EntityTypeBuilder<T> builder) where T : class, ICreateAuditable
        {
            // Relationships
            builder.HasOne(s => s.Creator)
                .WithMany()
                .HasForeignKey(d => d.CreatorId);
        }
    }
}
