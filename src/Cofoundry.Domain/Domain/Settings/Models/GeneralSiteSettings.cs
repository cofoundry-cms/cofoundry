namespace Cofoundry.Domain;

public class GeneralSiteSettings : ICofoundrySettings
{
    public string ApplicationName { get; set; }
    public bool AllowAutomaticUpdates { get; set; }
}
