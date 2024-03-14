using Microsoft.AspNetCore.Html;

namespace Cofoundry.Web.Admin;

/// <summary>
/// Default implementation of <see cref="IStaticResourceReferenceRenderer"/>.
/// </summary>
public class StaticResourceReferenceRenderer : IStaticResourceReferenceRenderer
{
    private readonly IStaticFilePathFormatter _staticFilePathFormatter;
    private readonly IStaticResourceFileProvider _staticResourceFileProvider;
    private readonly DebugSettings _debugSettings;

    public StaticResourceReferenceRenderer(
        IStaticFilePathFormatter staticFilePathFormatter,
        IStaticResourceFileProvider staticResourceFileProvider,
        DebugSettings debugSettings
        )
    {
        _staticFilePathFormatter = staticFilePathFormatter;
        _staticResourceFileProvider = staticResourceFileProvider;
        _debugSettings = debugSettings;
    }

    /// <inheritdoc/>
    public string JsPath(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        string virtualPath = JsPathWithoutVersion(moduleRouteLibrary, fileName);

        return _staticFilePathFormatter.AppendVersion(virtualPath);
    }

    /// <inheritdoc/>
    public string CssPath(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        var virtualPath = CssPathWithoutVersion(moduleRouteLibrary, fileName);

        return _staticFilePathFormatter.AppendVersion(virtualPath);
    }

    /// <inheritdoc/>
    public HtmlString ScriptTag(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        var jsPath = JsPath(moduleRouteLibrary, fileName);

        return FormatScriptTag(jsPath);
    }

    /// <inheritdoc/>
    public HtmlString ScriptTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        var jsPath = JsPathWithoutVersion(moduleRouteLibrary, fileName);

        if (!FileExists(jsPath)) return HtmlString.Empty;

        var jsPathWithVersion = _staticFilePathFormatter.AppendVersion(jsPath);
        return FormatScriptTag(jsPathWithVersion);
    }

    /// <inheritdoc/>
    public IEnumerable<HtmlString> ScriptTagsForDirectory(ModuleRouteLibrary moduleRouteLibrary)
    {
        var directoryScripts = _staticResourceFileProvider
            .GetDirectoryContents(moduleRouteLibrary.JsDirectory())
            .Select(f => ReduceResourceName(f.Name))
            .Distinct();

        var formattedScripts = directoryScripts
            .Select(f => ScriptTag(moduleRouteLibrary, "js/" + f));

        return formattedScripts;
    }

    /// <inheritdoc/>
    public HtmlString CssTag(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        var cssPath = CssPath(moduleRouteLibrary, fileName);

        return FormatCssTag(cssPath);
    }

    /// <inheritdoc/>
    public HtmlString CssTagIfExists(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        var cssPath = CssPathWithoutVersion(moduleRouteLibrary, fileName);

        if (!FileExists(cssPath)) return HtmlString.Empty;

        var cssPathWithVersion = _staticFilePathFormatter.AppendVersion(cssPath);
        return FormatCssTag(cssPathWithVersion);
    }

    /// <inheritdoc/>
    public IEnumerable<HtmlString> CssTagsForDirectory(ModuleRouteLibrary moduleRouteLibrary)
    {
        var directoryScripts = _staticResourceFileProvider
            .GetDirectoryContents(moduleRouteLibrary.CssDirectory())
            .Select(f => ReduceResourceName(f.Name))
            .Distinct();

        var formattedScripts = directoryScripts
            .Select(f => CssTag(moduleRouteLibrary, "css/" + f));

        return formattedScripts;
    }

    private static HtmlString FormatScriptTag(string jsPath)
    {
        return new HtmlString($"<script src=\"{jsPath}\"></script>");
    }

    private static HtmlString FormatCssTag(string cssPath)
    {
        return new HtmlString($"<link href=\"{cssPath}\" rel=\"stylesheet\">");
    }

    /// <summary>
    /// Reduces a resource filename to remove the extension
    /// and any minification suffix.
    /// </summary>
    private static string ReduceResourceName(string fileName)
    {
        var name = Path.GetFileNameWithoutExtension(fileName);

        return StringHelper.RemoveSuffix(name, "_min");
    }

    private static string CssPathWithoutVersion(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        fileName = Path.GetFileNameWithoutExtension(fileName);
        var virtualPath = moduleRouteLibrary.CssFile(fileName);

        return virtualPath;
    }

    private string JsPathWithoutVersion(ModuleRouteLibrary moduleRouteLibrary, string fileName)
    {
        fileName = Path.GetFileNameWithoutExtension(fileName);
        string? virtualPath = null;

        // check for a minified resource first
        if (!_debugSettings.UseUncompressedResources)
        {
            var minPath = moduleRouteLibrary.JsFile(fileName + "_min");

            if (FileExists(minPath))
            {
                virtualPath = minPath;
            }
        }

        virtualPath ??= moduleRouteLibrary.JsFile(fileName);

        return virtualPath;
    }

    private bool FileExists(string virtualPath)
    {
        return _staticResourceFileProvider.GetFileInfo(virtualPath).Exists;
    }
}
