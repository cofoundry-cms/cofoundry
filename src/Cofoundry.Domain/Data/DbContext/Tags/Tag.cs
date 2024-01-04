namespace Cofoundry.Domain.Data;

public class Tag
{
    public int TagId { get; set; }

    public string TagText { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }
}
