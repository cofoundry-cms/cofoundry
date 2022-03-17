namespace Cofoundry.Core.AutoUpdate;

public class ExecuteDbServerScriptCommand
{
    public string Script { get; set; }
    public DbConnectionInfo ConnectionInfo { get; set; }
}
