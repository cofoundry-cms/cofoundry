namespace Cofoundry.Domain;

public interface IAdminModuleRegistration
{
    IEnumerable<AdminModule> GetModules();
}
