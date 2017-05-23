using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cofoundry.Web.Admin
{
    public class AngularTemplateBundleTransform : IBundleTransform
    {
        private readonly string _moduleName;

        public AngularTemplateBundleTransform(string moduleName)
        {
            _moduleName = moduleName;
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            var strBundleResponse = new StringBuilder();
            // Javascript module for Angular that uses templateCache 
            strBundleResponse.AppendFormat(
                @"angular.module('{0}').run(['$templateCache',function(t){{",
                _moduleName);

            foreach (var file in response.Files)
            {
                // Get the partial page, remove line feeds and escape quotes
                string content;
                using (var stream = file.VirtualFile.Open())
                using (var reader = new StreamReader(stream))
                {
                    var s = reader.ReadToEnd();
                    content = Regex
                        .Replace(s, @"\t|\n|\r", "")
                        .Replace("'", "\\'");
                }
                // Create insert statement with template
                strBundleResponse.AppendFormat(
                    @"t.put('{0}','{1}');", file.VirtualFile.VirtualPath, content);
            }
            strBundleResponse.Append(@"}]);");

            // Set the files to empty, because when we're not bundling angular requests the files directly by path.
            response.Files = Enumerable.Empty<BundleFile>();
            response.Content = strBundleResponse.ToString();
            response.ContentType = "text/javascript";
        }
    }
}