using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cofoundry.Domain.Tests.Integration
{
    public class TestHostEnvironemnt : IWebHostEnvironment
    {
        public TestHostEnvironemnt()
        {
            ApplicationName = typeof(TestHostEnvironemnt).Namespace;
            var rootPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            ContentRootPath = new Uri(Path.GetDirectoryName(rootPath)).LocalPath;
            ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
            EnvironmentName = "Test";
        }

        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
    }
}
