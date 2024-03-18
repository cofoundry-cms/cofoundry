﻿using Cofoundry.Core.AutoUpdate;
using System.Reflection;

namespace Cofoundry.Domain.Installation;

/// <summary>
/// Factory for creating all packages that will be used at 
/// startup to install/update the application.
/// </summary>
public class CofoundryUpdatePackageFactory : IUpdatePackageFactory
{
    public IEnumerable<UpdatePackage> Create(ICollection<ModuleVersion> versionHistory)
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

    private IEnumerable<IAlwaysRunUpdateCommand> GetAlwaysUpdateCommand()
    {
        yield return new RegisterPermissionsAndRolesUpdateCommand();
        yield return new RegisterPageTemplatesAndPageBlockTypesCommand();
    }

    private IEnumerable<IVersionedUpdateCommand> GetAdditionalCommands(ModuleVersion moduleVersion)
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
