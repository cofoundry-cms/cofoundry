using System.Text.Json;
using SimpleExec;

namespace Cofoundry.Build.Utilities;

public static class GitVersion
{
    public static async Task<GitVersionInfo> PatchAssemblyVersion()
    {
        var (output, err) = await Command.ReadAsync("dotnet", """dotnet-gitversion -config "eng/Cofoundry.Build/GitVersion/GitVersion.yml"  -updateprojectfiles""");

        var gitVersionInfo = JsonSerializer.Deserialize<GitVersionInfo>(output);

        if (gitVersionInfo == null || string.IsNullOrEmpty(gitVersionInfo.InformationalVersion))
        {
            throw new Exception($"Could not read gitversion info from output: {output}");
        }

        return gitVersionInfo;
    }
}

