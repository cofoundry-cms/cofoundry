#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool "nuget:?package=GitVersion.CommandLine"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

GitVersion versionInfo = null;
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
    GitVersion(new GitVersionSettings{
        UpdateAssemblyInfo = false,
        OutputType = GitVersionOutput.BuildServer
    });

    versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });

    Information("Building version {0} of Cofoundry.", versionInfo.NuGetVersion);

    var file = "./src/SolutionInfo.cs";
	CreateAssemblyInfo(file, new AssemblyInfoSettings {
		Version = versionInfo.AssemblySemVer,
		FileVersion = versionInfo.MajorMinorPatch + ".0",
		InformationalVersion = versionInfo.InformationalVersion,
		Copyright = "Copyright © Cofoundry.org 2016"
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
    // Git Link: http://www.michael-whelan.net/continuous-delivery-github-cake-gittools-appveyor/
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
    var nugets = GetFiles("../build/artifacts/*.nupkg");

    NuGetPush(nugets, new NuGetPushSettings {
        Source = "https://nuget.org/",
        ApiKey = "TODO"
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default").IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
