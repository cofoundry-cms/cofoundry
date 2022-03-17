namespace Cofoundry.Core.AutoUpdate;

public class UpdateDbSchemaCommand
{
    public string Sql { get; set; }
    public int Version { get; set; }
}
