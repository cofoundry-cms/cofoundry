namespace Cofoundry.Domain;

public class UpdateSeoSettingsCommand : IPatchableCommand, ILoggableCommand
{
    public string RobotsTxt { get; set; }

    public string HumansTxt { get; set; }
}
