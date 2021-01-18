using Cofoundry.Web.Admin.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Cofoundry.Web.Admin.Tests.VisuaEditor
{
    public class HtmlDocumentScriptInjectorTests
    {
        private static string _headScript;
        private static string _bodyScript;

        public HtmlDocumentScriptInjectorTests()
        {
            _headScript = LoadFile("HeadScript");
            _bodyScript = LoadFile("BodyScript");
        }

        [Theory]
        [InlineData("EmptyHeadAndBody", null)]
        [InlineData("EmptyHeadAndBody", "")]
        public void InjectScripts_WhenNoScripts_NotAltered(string fileName, string injectData)
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
            var input =  LoadFile(fileName);
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

        #region helpers

        private string LoadFile(string name)
        {
            const string RESOURCE_POSTFIX = ".html";
            const string RESOURCE_PREFIX = "Cofoundry.Web.Admin.Tests.Admin.Modules.VisuaEditor.Resources.";

            using (var stream = GetType().Assembly.GetManifestResourceStream(RESOURCE_PREFIX + name + RESOURCE_POSTFIX))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        #endregion
    }
}
