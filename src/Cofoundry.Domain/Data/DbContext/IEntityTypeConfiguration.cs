using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    public interface IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        void Create(EntityTypeBuilder<TEntity> builder);
    }
}
