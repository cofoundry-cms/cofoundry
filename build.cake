#tool "nuget:?package=GitVersion.CommandLine"

using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var pushPackages = Argument("PushPackages", "false") == "true";
bool isPrerelease = false;
GitVersion versionInfo = null;

var projectsToBuild = new string[]{
    "./src/Cofoundry.Core/Cofoundry.Core.csproj",
    "./src/Cofoundry.Domain/Cofoundry.Domain.csproj",
    "./src/Cofoundry.Web/Cofoundry.Web.csproj",
    "./src/Cofoundry.Web.Admin/Cofoundry.Web.Admin.csproj"
};


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

//GitVersion versionInfo = null;
var nugetPackageDir = Directory("./artifacts");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(nugetPackageDir);
    CleanDirectories("./src/**/bin/" + configuration);
    CleanDirectories("./src/**/obj/" + configuration);
});


Task("Patch-Assembly-Version")
    .IsDependentOn("Clean")
    .Does(() =>
{
    versionInfo = GitVersion(new GitVersionSettings{
        UpdateAssemblyInfo = false
    });

    Information("Building version {0} of Cofoundry.", versionInfo.InformationalVersion);

    isPrerelease = versionInfo.PreReleaseNumber.HasValue;

    // Patch the version number so it's picked up when dependent projects are references
    // as nuget dependencies. Can't find a better way to do this.
    // Also this needs to run before DotNetCoreRestore for some reason (cached?)
    var file = MakeAbsolute(File("./src/Directory.Build.props"));
    var xml = System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);
    xml = Regex.Replace(xml, @"(<Version>)(.+)(<\/Version>)", "${1}" +  versionInfo.NuGetVersion +"${3}");
    System.IO.File.WriteAllText(file.FullPath, xml, Encoding.UTF8);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Patch-Assembly-Version")
    .Does(() =>
{
    foreach (var projectToBuild in projectsToBuild)
    {
        DotNetCoreRestore(projectToBuild);
    }
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    
    var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            ArgumentCustomization = args => args
                .Append("/p:NuGetVersion=" + versionInfo.NuGetVersion)
                .Append("/p:AssemblyVersion=" + versionInfo.AssemblySemVer)
                .Append("/p:FileVersion=" + versionInfo.MajorMinorPatch + ".0")
                .Append("/p:InformationalVersion=" + versionInfo.InformationalVersion)
                .Append("/p:Copyright=" + "\"Copyright © Cofoundry Technologies Ltd " + DateTime.Now.Year + "\"")
        };
    
    foreach (var projectToBuild in projectsToBuild)
    {
        DotNetCoreBuild(projectToBuild, settings);
    }
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = "./artifacts/",
            NoBuild = true
        };
    
    foreach (var projectToBuild in projectsToBuild)
    {
        DotNetCorePack(projectToBuild, settings);
    }
});

Task("PushNuGetPackage")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var nugets = GetFiles("./artifacts/*.nupkg");
    
    if (pushPackages)
    {
        Information("Pushing packages");
        
        if (isPrerelease)
        {
            NuGetPush(nugets, new NuGetPushSettings {
                Source = "https://www.myget.org/F/cofoundry/api/v2/package",
                ApiKey = EnvironmentVariable("MYGET_API_KEY")
            });
        }
        else
        {
            NuGetPush(nugets, new NuGetPushSettings {
                Source = "https://nuget.org/",
                ApiKey = EnvironmentVariable("NUGET_API_KEY")
            });
        }
    }
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default").IsDependentOn("PushNuGetPackage");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
