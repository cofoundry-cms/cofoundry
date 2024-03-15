using Cofoundry.Web.Admin.Internal;
using System.Text;

namespace Cofoundry.Web.Admin.Tests.VisualEditor;

public class HtmlDocumentScriptInjectorTests
{
    private static readonly string _headScript;
    private static readonly string _bodyScript;

    static HtmlDocumentScriptInjectorTests()
    {
        _headScript = LoadFile("HeadScript");
        _bodyScript = LoadFile("BodyScript");
    }

    [Theory]
    [InlineData("EmptyHeadAndBody", null)]
    [InlineData("EmptyHeadAndBody", "")]
    public void InjectScripts_WhenNoScripts_NotAltered(string fileName, string? injectData)
    {
        var input = LoadFile(fileName);
        var htmlDocumentScriptInjector = new HtmlDocumentScriptInjector();

        var result = htmlDocumentScriptInjector.InjectScripts(input, injectData, injectData);

        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("EmptyHeadAndBody")]
    [InlineData("HeadBodyAndContent")]
    [InlineData("NoHead")]
    public void InjectScripts_WhenValidHtml_InjectsScripts(string fileName)
    {
        var input = LoadFile(fileName);
        var expected = LoadFile(fileName + "Output");
        var htmlDocumentScriptInjector = new HtmlDocumentScriptInjector();

        var result = htmlDocumentScriptInjector.InjectScripts(input, _headScript, _bodyScript);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void InjectScripts_XML_NotAltered()
    {
        var input = LoadFile("XsltDocument");
        var htmlDocumentScriptInjector = new HtmlDocumentScriptInjector();

        var result = htmlDocumentScriptInjector.InjectScripts(input, _headScript, _bodyScript);

        Assert.Equal(input, result);
    }

    [Theory]
    [InlineData("JSON")]
    [InlineData("InvalidStructure")]
    public void InjectScripts_WhenInvalidHtml_NotAltered(string fileName)
    {
        var input = LoadFile(fileName);
        var htmlDocumentScriptInjector = new HtmlDocumentScriptInjector();

        var result = htmlDocumentScriptInjector.InjectScripts(input, _headScript, _bodyScript);

        Assert.Equal(input, result);
    }

    private static string LoadFile(string name)
    {
        const string RESOURCE_POSTFIX = ".html";
        const string RESOURCE_PREFIX = "Cofoundry.Web.Admin.Tests.Admin.Modules.VisualEditor.Resources.";

        var filename = RESOURCE_PREFIX + name + RESOURCE_POSTFIX;
        var assembly = typeof(HtmlDocumentScriptInjectorTests).Assembly;

        using var stream = assembly.GetManifestResourceStream(filename);
        var names = assembly.GetManifestResourceNames();
        if (stream == null)
        {
            throw new FileNotFoundException($"Could not find embedded resource {filename}", filename);
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
