namespace Cofoundry.Build.Utilities;

/// <summary>
/// <see href="https://gitversion.net/docs/reference/variables"/>
/// </summary>
public class GitVersionInfo
{
    /// <summary>
    /// Suitable for .NET AssemblyInformationalVersion. Defaults to FullSemVer suffixed by
    /// FullBuildMetaData e.g. "0.12.2-spike-mono-repo.1+9.Branch.spike-mono-repo.Sha.670c0d970c06440c70290b63d5447d6e7a52c18b"
    /// </summary>
    public string InformationalVersion { get; set; } = string.Empty;

    /// <summary>
    /// Major, Minor and Patch joined together, separated by "." e.g. "1.2.3".
    /// </summary>
    public string MajorMinorPatch { get; set; } = string.Empty;

    /// <summary>
    /// Value to use as the assembly "FileVersion" e.g. "1.2.3.0".
    /// </summary>
    public string FileVersion => MajorMinorPatch + ".0";

    /// <summary>
    /// The semantical version number, including PreReleaseTagWithDash for pre-release version numbers.
    /// This is the version assigned to the NuGet package e.g. "0.12.2" for release or
    /// "0.12.2-bugfix-560-admin-not-loading.1" for prereleases.
    /// </summary>
    public string SemVer { get; set; } = string.Empty;

    /// <summary>
    /// If this is a pre-release (commit is not tagged), this contains the
    /// number of that release (number of commits since last tag).
    /// </summary>
    public int? PreReleaseNumber { get; set; }

    /// <summary>
    /// True if this is a pre-release version i.e. the commit is not tagged with
    /// a version number. 
    /// </summary>
    public bool IsPreRelease => PreReleaseNumber.HasValue;
}
