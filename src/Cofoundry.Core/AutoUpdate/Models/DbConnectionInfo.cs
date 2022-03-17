namespace Cofoundry.Core.AutoUpdate;

public class DbConnectionInfo
{
    public string ServerAddress { get; set; }
    public string DbName { get; set; }
    public bool IsWindowsAuthentication { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool IsAzure { get; set; }
}
