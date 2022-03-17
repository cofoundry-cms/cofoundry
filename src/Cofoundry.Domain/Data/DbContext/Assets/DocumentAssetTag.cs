namespace Cofoundry.Domain.Data;

public class DocumentAssetTag : ICreateAuditable, IEntityTag
{
    public int DocumentAssetId { get; set; }
    public int TagId { get; set; }
    public virtual DocumentAsset DocumentAsset { get; set; }
    public virtual Tag Tag { get; set; }

    public System.DateTime CreateDate { get; set; }
    public int CreatorId { get; set; }
    public virtual User Creator { get; set; }
}
