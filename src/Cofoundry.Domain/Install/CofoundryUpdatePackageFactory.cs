using System.Reflection;
using Cofoundry.Core.AutoUpdate;

namespace Cofoundry.Domain.Installation;

/// <summary>
/// Factory for creating all packages that will be used at 
/// startup to install/update the application.
/// </summary>
public class CofoundryUpdatePackageFactory : IUpdatePackageFactory
{
    public IEnumerable<UpdatePackage> Create(IReadOnlyCollection<ModuleVersion> versionHistory)
    {
        var moduleVersion = versionHistory.SingleOrDefault(m => m.Module == CofoundryModuleInfo.ModuleIdentifier);

        var dbCommandFactory = new DbUpdateCommandFactory();
        var commands = new List<IVersionedUpdateCommand>();
        commands.AddRange(dbCommandFactory.Create(GetType().GetTypeInfo().Assembly, moduleVersion));
        commands.AddRange(GetAdditionalCommands(moduleVersion));

        var package = new UpdatePackage()
        {
            VersionedCommands = commands,
            AlwaysUpdateCommands = GetAlwaysUpdateCommand().ToList(),
            ModuleIdentifier = CofoundryModuleInfo.ModuleIdentifier
        };

        yield return package;
    }

    private static IEnumerable<IAlwaysRunUpdateCommand> GetAlwaysUpdateCommand()
    {
        yield return new RegisterPermissionsAndRolesUpdateCommand();
        yield return new RegisterPageTemplatesAndPageBlockTypesCommand();
    }

    private static IEnumerable<IVersionedUpdateCommand> GetAdditionalCommands(ModuleVersion? moduleVersion)
    {
        if (moduleVersion == null)
        {
            var createDirectoriesCommand = new CreateDirectoriesUpdateCommand()
            {
                Version = 1,
                Description = "InitCofoundryDirectories",
                Directories = new string[]
                {
                    "~/App_Data/Files/Images/",
                    "~/App_Data/Files/Other/",
                    "~/App_Data/Emails/"
                }
            };

            yield return createDirectoriesCommand;
        }

        var importPermissionsCommand = new ImportPermissionsCommand();
        if (moduleVersion == null || moduleVersion.Version < importPermissionsCommand.Version)
        {
            yield return new ImportPermissionsCommand();
        }
    }
}
