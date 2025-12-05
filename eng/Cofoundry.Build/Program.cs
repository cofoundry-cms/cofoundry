using System.Diagnostics.CodeAnalysis;
using Cofoundry.Build;
using Cofoundry.Build.Utilities;
using Cofoundry.Core;
using Cofoundry.DocGenerator;
using Microsoft.Extensions.FileSystemGlobbing;
using static Bullseye.Targets;
using static SimpleExec.Command;

const string ArtifactDirectory = "artifacts";
const string TemplateProjectDirectory = "./templates/Cofoundry.Templates";
const string TemplateProjectPath = $"{TemplateProjectDirectory}/Cofoundry.Templates.csproj";

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

Target("patch-template-projects", dependsOn: ["build"], async () =>
{
    ValidateVersionInfoRead(_versionInfo);

    // patch template sub-project csproj files as they have different build 
    // requirements to normal projects
    Matcher matcher = new();
    matcher.AddIncludePatterns(["*/**/*.csproj"]);

    var projectFilePaths = matcher.GetResultsInFullPath(TemplateProjectDirectory);

    foreach (var projectFilePath in projectFilePaths)
    {
        _logger.WriteLine($"Patching template project: {projectFilePath}");

        var projectFile = new ProjectFile(projectFilePath);

        projectFile.ConvertProjectReferencesToNuGet(_versionInfo.SemVer, _logger);
        projectFile.RemoveVersionProperties();
        projectFile.Save();
    }
});

Target("pack", dependsOn: ["build", "patch-template-projects"], async () =>
{
    ValidateVersionInfoRead(_versionInfo);

    // pack solution projects
    await RunAsync("dotnet", $"pack -c Release -o artifacts --no-build");

    // pack template projects separately as they aren't included in the solution
    await RunAsync("dotnet", $"pack {TemplateProjectPath} -c Release -o artifacts --no-build");
});

Target("publish-packages", dependsOn: ["test", "pack"], () =>
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

Target("publish-docs", dependsOn: ["patchAssemblyVersion"], async () =>
{
    ValidateVersionInfoRead(_versionInfo);

    var forcePublish = Environment.GetEnvironmentVariable("DOCS_FORCE_PUBLISH");

    if (_versionInfo.IsPreRelease && !BoolParser.ParseOrDefault(forcePublish))
    {
        _logger.WriteLine("Skipping docs publish because this is a pre-release version.");
        return;
    }

    var blobConnectionString = Environment.GetEnvironmentVariable("DOCS_STORAGE_CONNECTION_STRING");
    var webhook = Environment.GetEnvironmentVariable("DOCS_COMPLETION_WEBHOOK");

    if (string.IsNullOrEmpty(blobConnectionString))
    {
        _logger.WriteLine("Docs storage connection string not found. Docs will be published to artifacts path only.");
    }

    _logger.WriteLine($"Publishing docs to version {_versionInfo.MajorMinorPatch}");

    var docGenerator = new DocGenerator(new()
    {
        BlobStorageConnectionString = blobConnectionString,
        CleanDestination = false,
        OnCompleteWebHook = webhook,
        UseAzure = !string.IsNullOrEmpty(blobConnectionString),
        SourcePath = "docs/user-docs/",
        OutputPath = $"{ArtifactDirectory}/user-docs/",
        Version = $"{_versionInfo.Major}.{_versionInfo.Minor}.0"
    });

    await docGenerator.GenerateAsync();
});

Target("publish", dependsOn: ["publish-packages", "publish-docs"]);

Target("default", dependsOn: ["test"]);

await RunTargetsAndExitAsync(args, messageOnly: ex => ex is SimpleExec.ExitCodeException);

static void ValidateVersionInfoRead([NotNull] GitVersionInfo? versionInfo)
{
    if (versionInfo == null)
    {
        throw new InvalidOperationException("Version info has not yet been fetched.");
    }
}
