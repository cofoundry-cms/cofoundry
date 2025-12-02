using System.Diagnostics.CodeAnalysis;
using Cofoundry.Build;
using Cofoundry.Build.Utilities;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string ArtifactDirectory = "artifacts";
const string TemplateProjectPath = "./templates/Cofoundry.Templates/Cofoundry.Templates.csproj";

var _logger = Console.Out;
GitVersionInfo? _versionInfo = null;

Target("init", () =>
{
    Run("dotnet", "tool restore");
});

Target("clean", () =>
{
    FileHelper.EnsureDirectoryExists(ArtifactDirectory);
    FileHelper.DeleteContents(ArtifactDirectory);
});

Target("patchAssemblyVersion", dependsOn: ["init", "clean"], async () =>
{
    _versionInfo = await GitVersion.PatchAssemblyVersion();

    _logger.WriteLine($"Building Cofoundry version: '{_versionInfo.InformationalVersion}', prerelease number {_versionInfo.PreReleaseNumber}");
});

Target("build", dependsOn: ["patchAssemblyVersion"], async () =>
{
    // build solution projects
    await RunAsync("dotnet", "build --configuration Release --nologo --verbosity quiet");
    // build templates separately as they aren't included in the solution
    await RunAsync("dotnet", $"build {TemplateProjectPath} --configuration Release --nologo --verbosity quiet");
});

Target("test", dependsOn: ["build"], async () =>
{
    await RunAsync("dotnet", "test --configuration Release --no-build --nologo --verbosity normal --logger trx");
});

Target("pack", dependsOn: ["build"], async () =>
{
    ValidateVersionInfoRead(_versionInfo);

    // pack solution projects
    await RunAsync("dotnet", $"pack -c Release -o artifacts --no-build");

    // pack templates separately as they aren't included in the sollution
    await RunAsync("dotnet", $"pack {TemplateProjectPath} -c Release -o artifacts --no-build");
});

Target("publish", dependsOn: ["test", "pack"], () =>
{
    ValidateVersionInfoRead(_versionInfo);

    var packagesToPush = Directory
        .GetFiles("artifacts", "*.nupkg", SearchOption.TopDirectoryOnly)
        .OrderBy(p => p.Contains("Cofoundry.Plugin"))
        .ThenBy(p => p)
        .ToArray();

    _logger.WriteLine($"Found {packagesToPush.Length} packages to publish:");

    foreach (var packageToPush in packagesToPush)
    {
        _logger.WriteLine(packageToPush);
    }

    string? url;
    string? apiKey;

    if (_versionInfo.IsPreRelease)
    {
        url = "https://www.myget.org/F/cofoundry/api/v2/package";
        apiKey = Environment.GetEnvironmentVariable("MYGET_API_KEY");
    }
    else
    {
        url = "https://nuget.org/";
        apiKey = Environment.GetEnvironmentVariable("NUGET_API_KEY");
    }

    if (string.IsNullOrWhiteSpace(apiKey))
    {
        _logger.WriteLine("API key not found, skipping push.");
        return;
    }

    _logger.WriteLine($"Pushing packages to {url}");

    foreach (var packageToPush in packagesToPush)
    {
        _logger.WriteLine($"Pushing package {packageToPush}");
        Run("dotnet", $"nuget push {packageToPush} -s {url} -k {apiKey}");
    }
});

Target("default", dependsOn: ["test"]);

await RunTargetsAndExitAsync(args, messageOnly: ex => ex is SimpleExec.ExitCodeException);

static void ValidateVersionInfoRead([NotNull] GitVersionInfo? _versionInfo)
{
    if (_versionInfo == null)
    {
        throw new InvalidOperationException("Version info has not yet been fetched.");
    }
}
