namespace Cofoundry.BasicTestSite.Cofoundry.PageExtensions;

public class PageExtensionRegistration : IPageExtensionRegistration
{
    public void RegisterPageExtensions(ExtensionRegistrationContext options)
    {
        options.Add<PageMetaDataDataModel>(o =>
        {
            //o.Name = "Meta";
            //o.GroupName = "Meta Data";
            //o.LoadProfile = EntityExtensionLoadProfile.Details;
        });
    }
}
