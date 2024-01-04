namespace Cofoundry.Domain.Data;

public class Setting
{
    public int SettingId { get; set; }

    public string SettingKey { get; set; } = string.Empty;

    public string SettingValue { get; set; } = string.Empty;

    public DateTime CreateDate { get; set; }

    public DateTime UpdateDate { get; set; }
}
