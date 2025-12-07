using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPASite.Data;

public class CatLikeMap : IEntityTypeConfiguration<CatLike>
{
    public void Configure(EntityTypeBuilder<CatLike> builder)
    {
        builder.ToTable(nameof(CatLike), DbConstants.DefaultAppSchema);

        builder.HasKey(e => new { e.CatCustomEntityId, e.UserId });
    }
}