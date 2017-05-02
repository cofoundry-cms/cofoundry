#tool "nuget:?package=GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var pushPackages = Argument("PushPackages", "false") == "true";
bool isPrerelease = false;
GitVersion versionInfo = null;

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
    CleanDirectory("./src/Cofoundry.Web/bin/");
    CleanDirectory("./src/Cofoundry.Web.Admin/bin/");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/Cofoundry.sln");
});

Task("Patch-Assembly-Version")
    .IsDependentOn("Clean")
    .Does(() =>
{
    versionInfo = GitVersion(new GitVersionSettings{
        UpdateAssemblyInfo = false
    });

    Information("Building version {0} of Cofoundry.", versionInfo.InformationalVersion);

    isPrerelease = !string.IsNullOrEmpty(versionInfo.PreReleaseNumber);

    var file = "./src/SolutionInfo.cs";
	CreateAssemblyInfo(file, new AssemblyInfoSettings {
		Version = versionInfo.AssemblySemVer,
		FileVersion = versionInfo.MajorMinorPatch + ".0",
		InformationalVersion = versionInfo.InformationalVersion,
		Copyright = "Copyright © Cofoundry.org " + DateTime.Now.Year
	});
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Patch-Assembly-Version")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./src/Cofoundry.sln", settings => settings.SetConfiguration(configuration));
    }
});

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    var nugetFilePaths = GetFiles("./src/Cofoundry.*/*.nuspec");
    
    var nuGetPackSettings = new NuGetPackSettings
    {   
        Version = versionInfo.NuGetVersion,
        OutputDirectory = nugetPackageDir,
        Verbosity = NuGetVerbosity.Detailed,
        ArgumentCustomization = args => args.Append("-Prop Configuration=" + configuration)
    };
    
    foreach (var path in nugetFilePaths)
    {
        Information("Packing:" + path);
        NuGetPack(path, nuGetPackSettings);
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

// Dont run anything yet
//RunTarget(target);
