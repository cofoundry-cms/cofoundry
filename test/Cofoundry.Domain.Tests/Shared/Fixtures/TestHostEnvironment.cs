using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Domain.Tests;

public class TestHostEnvironment : IWebHostEnvironment
{
    public TestHostEnvironment()
    {
        ApplicationName = typeof(TestHostEnvironment).Namespace ?? "Unknown Namespace";
        var rootPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var directory = Path.GetDirectoryName(rootPath);
        if (directory == null)
        {
            throw new Exception($"Assembly path '{rootPath}' did not translate to a directory path.");
        }
        ContentRootPath = new Uri(directory).LocalPath;
        ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
        EnvironmentName = "Test";
        WebRootFileProvider = new NullFileProvider();
    }

    public string ApplicationName { get; set; }
    public IFileProvider ContentRootFileProvider { get; set; }
    public string ContentRootPath { get; set; }
    public string EnvironmentName { get; set; }
    public IFileProvider WebRootFileProvider { get; set; }
    public string WebRootPath { get; set; } = string.Empty;
}
