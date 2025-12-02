using System.Text.RegularExpressions;
using System.Xml;

namespace Cofoundry.Build;

/// <summary>
/// Encapsulates various amendments that need to be made to a .NET csproj XML file.
/// </summary>
public class ProjectFile
{
    private readonly XmlDocument _xml;
    private readonly XmlElement _rootNode;
    private readonly string _filePath;

    public ProjectFile(string filePath)
    {
        var doc = new XmlDocument();
        doc.Load(filePath);

        _xml = doc;

        if (doc.DocumentElement == null)
        {
            throw new Exception($"Invalid csproj file, no root element found. File: {filePath}");
        }

        _rootNode = doc.DocumentElement;
        _filePath = filePath;
    }

    /// <summary>
    /// Removes any project versioning properties that wre uneccessarily applied
    /// via the GitVerison project file patching process.
    /// </summary>
    public void RemoveVersionProperties()
    {
        RemoveProperty("AssemblyVersion");
        RemoveProperty("FileVersion");
        RemoveProperty("InformationalVersion");
        RemoveProperty("Version");
    }

    /// <summary>
    /// Removes the specified property in the PropertyGroup XML node
    /// if it exists.
    /// </summary>
    public void RemoveProperty(string property)
    {
        var node = _xml.SelectSingleNode($"//PropertyGroup/{property}");
        node?.ParentNode?.RemoveChild(node);
    }

    /// <summary>
    /// Converts any path-based Cofoundry project references to a corresponding
    /// NuGet package reference. This is required for template sub-projects because
    /// the NuGet pack process does not automatically change the references.
    /// </summary>
    public void ConvertProjectReferencesToNuGet(string nugetVersion, TextWriter logger)
    {
        var projectReferenceNodes = _rootNode.SelectNodes("//ProjectReference");
        List<string> projects = [];

        if (projectReferenceNodes == null)
        {
            logger.WriteLine($"No project references found. Skipping reference patching for project: {_filePath}");
            return;
        }
        else
        {
            for (var i = projectReferenceNodes.Count - 1; i >= 0; i--)
            {
                var node = projectReferenceNodes[i]!;
                var projectPath = node.Attributes?["Include"]?.Value;

                if (string.IsNullOrEmpty(projectPath))
                {
                    continue;
                }

                var match = Regex.Match(projectPath, @".+[\\\/](.+).csproj");
                if (match.Success && match.Groups[1].Value.StartsWith("Cofoundry."))
                {
                    node.ParentNode!.RemoveChild(node);
                    projects.Add(match.Groups[1].Value);
                }
            }
        }

        if (projects.Count == 0)
        {
            throw new Exception($"Expected template project to reference at least one Cofoundry package, but no references found. File: {_filePath}");
        }

        var group = _xml.CreateElement("ItemGroup");
        _rootNode.AppendChild(group);

        foreach (var project in projects)
        {
            logger.WriteLine($"Patching reference to '{project}' to NuGet {nugetVersion}");

            var nugetRef = _xml.CreateElement("PackageReference");
            var includeAttr = _xml.CreateAttribute("Include");
            includeAttr.Value = project;
            nugetRef.Attributes.Append(includeAttr);
            var versionAttr = _xml.CreateAttribute("Version");
            versionAttr.Value = nugetVersion;
            nugetRef.Attributes.Append(versionAttr);

            group.AppendChild(nugetRef);
        }

    }

    public void Save()
    {
        _xml.Save(_filePath);
    }
}
