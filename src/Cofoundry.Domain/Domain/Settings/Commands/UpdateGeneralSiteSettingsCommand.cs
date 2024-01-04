namespace Cofoundry.Domain;

public class UpdateGeneralSiteSettingsCommand : IPatchableCommand, ILoggableCommand
{
    [Required]
    [MaxLength(100)]
    public string ApplicationName { get; set; } = string.Empty;

    public bool AllowAutomaticUpdates { get; set; }
}
