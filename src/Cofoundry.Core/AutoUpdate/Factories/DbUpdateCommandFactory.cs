using System.Reflection;

namespace Cofoundry.Core.AutoUpdate;

/// <summary>
/// Creates DbUpateCommands from sql scripts embedded in an assembly.
/// </summary>
public class DbUpdateCommandFactory
{
    /// <summary>
    /// Creates DbUpateCommands from sql scripts embedded in an assembly. Only new schema updates
    /// are included and functions/sps/triggers etc are only returned if there are any relevant schema
    /// updates, so you need to create a schema update file if you want to force these to update.
    /// </summary>
    /// <param name="assembly">The assembly to scan for sql scripts.</param>
    /// <param name="currentVersion">
    /// The current version of the module. May be <see langword="null"/> if
    /// no modules are installed.
    /// </param>
    /// <param name="scriptPath">The folder path of the script files which defaults to 'Install.Db.' (which equates to 'Install/Db/')</param>
    /// <returns>Collecton of IUpdateCommands that represents all the required db updates</returns>
    public IReadOnlyCollection<IVersionedUpdateCommand> Create(
        Assembly assembly,
        ModuleVersion? currentVersion,
        string scriptPath = "Install.Db."
        )
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var scriptFiles = GetScripts(assembly, scriptPath);
        var commands = new List<IVersionedUpdateCommand>();
        var maxVersionNumber = 0;

        // Get schema scripts we need so we know the version number.
        foreach (var scriptFile in scriptFiles.Where(s => s.Contains(".Schema.")))
        {
            var fileName = GetScriptFileName(scriptFile);
            var version = IntParser.ParseOrNull(fileName);
            if (!version.HasValue)
            {
                throw new InvalidCastException("Unable to parse version number from schema update file: " + scriptFile);
            }

            if (currentVersion != null && version.Value <= currentVersion.Version)
            {
                continue;
            }

            var command = new UpdateDbCommand()
            {
                Version = version.Value,
                ScriptType = DbScriptType.Schema,
                Sql = GetResource(assembly, scriptFile),
                Description = scriptFile,
                FileName = fileName
            };
            commands.Add(command);

            if (maxVersionNumber < version.Value)
            {
                maxVersionNumber = version.Value;
            }
        }

        if (commands.Count == 0)
        {
            return Array.Empty<IVersionedUpdateCommand>();
        }

        foreach (var scriptFile in scriptFiles.Where(s => !s.Contains(".Schema.")))
        {
            var command = new UpdateDbCommand()
            {
                Version = maxVersionNumber,
                Sql = GetResource(assembly, scriptFile),
                ScriptType = MapScriptType(scriptFile),
                Description = scriptFile,
                FileName = GetScriptFileName(scriptFile)
            };
            commands.Add(command);
        }

        return commands;
    }

    private static string GetScriptFileName(string path)
    {
        var dbScriptTypeNames = Enum.GetNames(typeof(DbScriptType));

        var startIndex = dbScriptTypeNames
            .Select(e => "." + e + ".")
            .Select(e => new
            {
                Index = path.LastIndexOf(e),
                PathPart = e
            })
            .Where(e => e.Index != -1)
            .Select(e => (int?)(e.Index + e.PathPart.Length))
            .SingleOrDefault();

        if (!startIndex.HasValue)
        {
            throw new ArgumentException("Invalid script path. Does not contain a DbScriptType in the path name", nameof(path));
        }
        var fileName = path.Substring(startIndex.Value);

        return Path.GetFileNameWithoutExtension(fileName);
    }

    private static string[] GetScripts(Assembly assembly, string scriptPath)
    {
        var scripts = assembly
            .GetManifestResourceNames()
            .Where(f => f.Contains(scriptPath) && f.EndsWith(".sql"))
            .OrderBy(f => f)
            .ToArray();

        return scripts;
    }

    private static DbScriptType MapScriptType(string scriptFile)
    {
        if (scriptFile.Contains(".Views."))
        {
            return DbScriptType.Views;
        }

        if (scriptFile.Contains(".Functions."))
        {
            return DbScriptType.Functions;
        }

        if (scriptFile.Contains(".StoredProcedures."))
        {
            return DbScriptType.StoredProcedures;
        }

        if (scriptFile.Contains(".Triggers."))
        {
            return DbScriptType.Triggers;
        }

        if (scriptFile.Contains(".Finalize."))
        {
            return DbScriptType.Finalize;
        }

        throw new NotSupportedException("Script type not recognised and will not be run: " + scriptFile);
    }

    /// <remarks>
    /// See http://www.codeproject.com/Articles/7432/Create-String-Variables-from-Embedded-Resources-Fi
    /// </remarks>
    private static string GetResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException($"Could not read embedded resource '{resourceName}' in assembly {assembly.FullName}.");
        }

        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
