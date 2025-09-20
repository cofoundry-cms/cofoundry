using System.Diagnostics.CodeAnalysis;
using Cofoundry.Build.Utilities;
using static Bullseye.Targets;
using static SimpleExec.Command;

var _logger = Console.Out;
GitVersionInfo? _versionInfo = null;

Target("init", () =>
{
    Run("dotnet", "tool restore");
});

Target("clean", () =>
{
    var directory = new DirectoryInfo("artifacts");
    directory.Create();
    foreach (var file in directory.GetFiles())
    {
        file.Delete();
    }

    foreach (var dir in directory.GetDirectories())
    {
        dir.Delete(true);
    }
});

Target("patchAssemblyVersion", dependsOn: ["init", "clean"], async () =>
{
    _versionInfo = await GitVersion.PatchAssemblyVersion();

    _logger.WriteLine("Building Cofoundry version: {0}", _versionInfo.InformationalVersion);
});

Target("build", dependsOn: ["patchAssemblyVersion"], async () =>
{
    await RunAsync("dotnet", "build --configuration Release --nologo --verbosity quiet");
});

Target("test", dependsOn: ["build"], async () =>
{
    await RunAsync("dotnet", "test --configuration Release --no-build --nologo --verbosity quiet");
});

Target("pack", dependsOn: ["build"], async () =>
{
    ValidateVersionInfoRead(_versionInfo);

    await RunAsync("dotnet", $"pack -c Release -o artifacts --no-build");
});

Target("publish", dependsOn: ["pack", "test"], () =>
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

    if (_versionInfo.IsPrerelease)
    {
        url = "https://www.myget.org/F/dh/api/v3/index.json";
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

Target("default", dependsOn: ["publish"]);

await RunTargetsAndExitAsync(args, messageOnly: ex => ex is SimpleExec.ExitCodeException);

static void ValidateVersionInfoRead([NotNull] GitVersionInfo? _versionInfo)
{
    if (_versionInfo == null)
    {
        throw new InvalidOperationException("Version info has not yet been fetched.");
    }
}
